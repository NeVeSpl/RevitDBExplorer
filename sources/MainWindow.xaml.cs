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
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Properties;
using RevitDBExplorer.ViewModels;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Command;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    internal partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<SnoopableCategoryTreeVM> treeItems = new();
        private ObservableCollection<SnoopableMember> listItems = new();
        private readonly CommandsVM commandsVM = new();
        private SnoopableMember listSelectedItem = null;
        private string listItemsFilterPhrase = string.Empty;
        private string treeItemsFilterPhrase = string.Empty;
        private string databaseQuery = string.Empty;
        private string databaseQueryToolTip = string.Empty;

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
        public CommandsVM CommandsVM
        {
            get
            {
                return commandsVM;
            }
            set
            {
                //commandsVM = value;
                OnPropertyChanged();
            }
        }
        public SnoopableMember ListViewSelectedItem
        {
            get
            {
                return listSelectedItem;
            }
            set
            {
                listSelectedItem = value;
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
                FilterListView();                
                OnPropertyChanged();
            }
        }
        public string TreeItemsFilterPhrase
        {
            get
            {
                return treeItemsFilterPhrase;
            }
            set
            {
                treeItemsFilterPhrase = value;
                FilterTreeView();
                OnPropertyChanged();
            }
        }
        public string DatabaseQuery
        {
            get
            {
                return databaseQuery;
            }
            set
            {
                databaseQuery = value;
                TryQueryDatabase(value);
                OnPropertyChanged();
            }
        }
        public string DatabaseQueryToolTip
        {
            get
            {
                return databaseQueryToolTip;
            }
            set
            {
                databaseQueryToolTip = value;               
                OnPropertyChanged();
            }
        }
        

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            var ver = GetType().Assembly.GetName().Version;
            var revit_ver = typeof(Autodesk.Revit.DB.Element).Assembly.GetName().Version;
            Title += $" 20{revit_ver.Major} - v{ver.Major}.{ver.Minor}.{ver.Build}";            
        }
        public MainWindow(IList<SnoopableObject> objects) : this()
        {
            PopulateTreeView(objects);
        }


        private async void SelectorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                ResetTreeItems();
                ResetListItems();
                ResetDatabaseQuery();

                var tag = ((Control)sender).Tag as string;
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
                if (e.NewValue is SnoopableObjectTreeVM snoopableObjectVM)
                {
                    var snoopableMembers = await ExternalExecutor.ExecuteInRevitContextAsync(x => snoopableObjectVM.Object.GetMembers(x).ToList());
                    PopulateListView(snoopableMembers);
                }
                else
                {
                    ResetListItems();
                }
            }
            catch (Exception ex)
            {
                ResetListItems();
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
        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ExternalExecutor.ExecuteInRevitContextAsync(uiApp =>
                {
                    foreach (var item in listItems)
                    {
                        item.ReadValue();
                    }
                    return true;
                });
               
            }
            catch (Exception ex)
            {
                ShowErrorMsg("SnoopableMember.ReadValue", ex);
            }
        }
        private async void TryQueryDatabase(string query)
        {
            try
            {
                ResetTreeItems();
                ResetListItems();

                var snoopableObjects = await ExternalExecutor.ExecuteInRevitContextAsync(uiApp =>
                {
                    var document = uiApp?.ActiveUIDocument?.Document;

                    if (document == null) return Enumerable.Empty<SnoopableObject>().ToList();

                    var result = RevitDatabaseQueryService.Parse(document, query);
                    DatabaseQueryToolTip = result.CollectorSyntax + ".ToElements();";
                    CommandsVM.Update(result.Commands);
                    return result.Collector.ToElements().Select(x => new SnoopableObject(x, document)).ToList();
                });
                PopulateTreeView(snoopableObjects);
            }
            catch (Exception ex)
            {
                ShowErrorMsg("RevitDatabaseQueryParser.Parse", ex);
            }
        }        

        
        private void PopulateTreeView(IList<SnoopableObject> objects)
        {
            var items = objects.GroupBy(x => x.TypeName).Select(x => new SnoopableCategoryTreeVM(x.Key, x, TreeViewFilter)).OrderBy(x => x.Name);

            TreeItems = new(items);
            if ((objects.Count > 0 && objects.Count < 29) || (TreeItems.Count() == 1))
            {
                // Expand
                foreach (var cat in TreeItems)
                {
                    cat.IsExpanded = true;
                    foreach (var sub in (cat.Items ?? Enumerable.Empty<TreeViewItemVM>()))
                    {
                        sub.IsExpanded = true;
                        foreach (var subsub in (sub.Items ?? Enumerable.Empty<TreeViewItemVM>()))
                        {
                            //subsub.IsExpanded = true;
                        }
                    }                   
                }       
                // Select
                var firstObject = TreeItems.First().Items.First();
                if (firstObject.Items?.Any() == true)
                {
                    firstObject.IsExpanded = true;
                    firstObject.Items.First().IsSelected = true;
                }
                firstObject.IsSelected = true;                
            }           
        }
        private void PopulateListView(IList<SnoopableMember> members)
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
                return snoopableMember.Name.IndexOf(listItemsFilterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return true;
        }
        private bool TreeViewFilter(object item)
        {
            if (item is SnoopableObjectTreeVM snoopableObjectVM)
            {
                return snoopableObjectVM.Object.Name.IndexOf(treeItemsFilterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return true;
        }
        
        private void FilterListView()
        {
            if (ListItems != null)
            {
                CollectionViewSource.GetDefaultView(ListItems).Refresh();
            }
        }
        private void FilterTreeView()
        {
            if (TreeItems != null)
            {
                foreach (var item in TreeItems)
                {
                    item.Refresh();
                }
            }
        }       
        
        private void ResetListItems()
        {
            PopulateListView(System.Array.Empty<SnoopableMember>());
        }
        private void ResetTreeItems()
        {
            treeItemsFilterPhrase = "";
            OnPropertyChanged(nameof(TreeItemsFilterPhrase));
            PopulateTreeView(System.Array.Empty<SnoopableObject>());             
        }
        private void ResetDatabaseQuery()
        {
            databaseQuery = "";
            OnPropertyChanged(nameof(DatabaseQuery));
            DatabaseQueryToolTip = "";
            commandsVM.Update(Enumerable.Empty<RDQCommand>());
        }

        private void TreeViewMenuItemInRevit_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var snoopableObject = (menuItem?.DataContext as SnoopableObjectTreeVM)?.Object;
            try
            {
                if (snoopableObject?.Object is not null)
                {
                    switch (menuItem.Tag)
                    {
                        case "Select":
                            RevitObjectPresenter.Select(snoopableObject);
                            break;
                        case "Isolate":
                            RevitObjectPresenter.Isolate(snoopableObject);
                            break;
                        case "Show":
                            RevitObjectPresenter.Show(snoopableObject);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMsg($"RevitObjectPresenter.{menuItem.Tag}", ex);
            }
        }
        private void TreeViewMenuItemSnoop_Click(object sender, RoutedEventArgs e)
        {
            var snoopableObject = ((sender as MenuItem)?.DataContext as SnoopableObjectTreeVM)?.Object;
            if (snoopableObject is not null)
            {
                var window = new MainWindow(new[] { snoopableObject });
                window.Owner = this;
                window.Show();
            }
        }
        private void TreeViewItem_MouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            (sender as TreeViewItem).IsSelected = true;
            //e.Handled = true;
        }
        private void ListViewMenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var menu = menuItem.Parent as ContextMenu;
            var item = menu.PlacementTarget as ListViewItem;

            if (item.DataContext is SnoopableMember snoopableMember)
            {
                Clipboard.SetDataObject($"{snoopableMember.Name}= {snoopableMember.Value}");
            }
        }
        private void ButtonWithSubMenu_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = ContextMenuService.GetContextMenu(sender as DependencyObject);
            if (contextMenu == null)
            {
                return;
            }
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.PlacementTarget = sender as UIElement;
            contextMenu.IsOpen = true;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.RevitWindowHandle.BringWindowToFront();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AppSettings.Default.MainWindowHeight = Height;
            AppSettings.Default.MainWindowWidth = Width;
            AppSettings.Default.Save();
        }

        private static void ShowErrorMsg(string title, Exception ex)
        {
            var dialog = new TaskDialog(title)
            {
                MainInstruction = Labels.GetLabelForException(ex),
                MainContent = ex.StackTrace,
                CommonButtons = TaskDialogCommonButtons.Ok,
                DefaultButton = TaskDialogResult.Ok,
                MainIcon = TaskDialogIcon.TaskDialogIconError
            };
            dialog.Show();
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