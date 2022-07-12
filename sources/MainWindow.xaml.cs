using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.ViewModels;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    internal partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<SnoopableCategoryTreeVM> treeItems = new();
        private ObservableCollection<SnoopableMember> listItems = new();
        private SnoopableMember listViewSelectedItem = null;
        private string listItemsFilterPhrase = String.Empty;

        public ObservableCollection<SnoopableCategoryTreeVM> TreeItems
        {
            get
            {
                return treeItems;
            }
            set
            {
                treeItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SnoopableMember> ListItems
        {
            get
            {
                return listItems;
            }
            set
            {
                listItems = value;
                OnPropertyChanged();
            }
        }     
        public SnoopableMember ListViewSelectedItem
        {
            get
            {
                return listViewSelectedItem;
            }
            set
            {
                listViewSelectedItem = value;
                OnPropertyChanged();
            }
        }
        public string ListItemsFilterPhrase
        {
            get
            {
                return listItemsFilterPhrase;
            }
            set
            {
                listItemsFilterPhrase = value;
                if (ListItems != null)
                {
                    CollectionViewSource.GetDefaultView(ListItems).Refresh();
                }
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public MainWindow(IList<SnoopableObject> objects) : this()
        {
            PopulateTreeView(objects);
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tag = ((Button)sender).Tag as string;
                var selector = (Selector)Enum.Parse(typeof(Selector), tag);
                if (selector == Selector.PickEdge || selector ==  Selector.PickFace)
                {
                    //this.WindowState = WindowState.Minimized;
                }
                var snoopableObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x => Selectors.Snoop(x, selector).ToList());
                if (selector == Selector.PickEdge || selector == Selector.PickFace)
                {
                    //this.WindowState = WindowState.Normal;
                }
                ListItems = null;
                PopulateTreeView(snoopableObjects);
            }
            catch (Exception ex)
            {
                ShowErrorMsg("Selectors.Snoop", ex);
            }
        }
        private async void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var snoopableObjectVM = e.NewValue as SnoopableObjectTreeVM;
                if (snoopableObjectVM != null)
                {                    
                    var snoopableMembers = await ExternalExecutor.ExecuteInRevitContextAsync(x => snoopableObjectVM.Object.GetMembers(x).ToList());
                    PopulateListView(snoopableMembers);
                }
            }
            catch (Exception ex)
            {
                ListItems = null;
                ShowErrorMsg( "SnoopableObject.GetMembers", ex);
            }
        }
        private async void ListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ListViewSelectedItem?.CanBeSnooped == true)
                {
                    var snoopableObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x => ListViewSelectedItem.Snooop(x).ToList());
                    var window = new MainWindow(snoopableObjects);
                    window.Owner = this;
                    window.Show();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMsg("SnoopableMember.Snooop", ex);
            }
        }
        private void ListViewMenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var menu = menuItem.Parent as ContextMenu;
            var item = menu.PlacementTarget as ListViewItem;
            var snoopableMember = item.DataContext as SnoopableMember;

            if (snoopableMember != null)
            {
                Clipboard.SetDataObject($"{snoopableMember.Name}= {snoopableMember.Value}");
            }
        }

        private static void ShowErrorMsg(string title, Exception ex)
        {
            var dialog = new TaskDialog(title);
            dialog.MainInstruction = Labels.GetLabelForException(ex);
            dialog.MainContent = ex.StackTrace;
            dialog.CommonButtons = TaskDialogCommonButtons.Ok;
            dialog.DefaultButton = TaskDialogResult.Ok;
            dialog.MainIcon = TaskDialogIcon.TaskDialogIconError;
            dialog.Show();            
        }
        private void PopulateTreeView(IList<SnoopableObject> objects)
        {
            var items = objects.GroupBy(x => x.TypeName).Select(x => new SnoopableCategoryTreeVM(x.Key, x)).OrderBy(x => x.Name);

            TreeItems = new(items);
            if ((objects.Count > 0 && objects.Count < 7) || (TreeItems.Count() == 1))
            {
                var category = TreeItems.First();
                category.IsExpanded = true;
                var firstObject = category.Items.First();
                if (firstObject.Items?.Any() == true)
                {
                    firstObject.IsExpanded = true;
                    firstObject.Items.First().IsSelected = true;
                }
                firstObject.IsSelected = true;                
            }
        }
        private void PopulateListView(List<SnoopableMember> members)
        {
            ListItems = new(members);

            var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(ListItems);
            
            lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SnoopableMember.DeclaringType)));          
            lcv.SortDescriptions.Add(new SortDescription(nameof(SnoopableMember.DeclaringTypeLevel), ListSortDirection.Ascending));
            lcv.SortDescriptions.Add(new SortDescription(nameof(SnoopableMember.MemberKind), ListSortDirection.Ascending));
            lcv.SortDescriptions.Add(new SortDescription(nameof(SnoopableMember.Name), ListSortDirection.Ascending));
            lcv.Filter = ListViewFilter;
        }

        private bool ListViewFilter(object item)
        {
            if (item is SnoopableMember snoopableMember)
            {
                if (snoopableMember.Name.IndexOf(listItemsFilterPhrase, StringComparison.OrdinalIgnoreCase) >= 0) return true;
                return false;
            }
            return true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.RevitWindowHandle.BringWindowToFront();
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion        
    }
}