using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Properties;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.QueryVisualization;
using RevitDBExplorer.UIComponents.Tree;
using RevitDBExplorer.WPF;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Command;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    internal partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<TreeViewItemVM> treeItems = new();        
        private readonly ListVM listVM = new();
        private readonly QueryVisualizationVM queryVisualizationVM = new();        
        private string listItemsFilterPhrase = string.Empty;
        private string treeItemsFilterPhrase = string.Empty;
        private string databaseQuery = string.Empty;
        private string databaseQueryToolTip = string.Empty;
        private bool isRevitBusy;
        private readonly DispatcherTimer isRevitBusyDispatcher;


        public ObservableCollection<TreeViewItemVM> TreeItems
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
        public ListVM List
        {
            get
            {
                return listVM;
            }
        }
        public QueryVisualizationVM QueryVisualization
        {
            get
            {
                return queryVisualizationVM;
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
                List.FilterListView();                
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
        public bool IsRevitBusy
        {
            get
            {
                return isRevitBusy;
            }
            set
            {
                if (isRevitBusy != value)
                {
                    isRevitBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        

        public MainWindow()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            InitializeComponent();
            this.DataContext = this;
            var ver = GetType().Assembly.GetName().Version;
            var revit_ver = typeof(Autodesk.Revit.DB.Element).Assembly.GetName().Version;
            Title += $" 20{revit_ver.Major} - {ver.ToGitHubTag()}";

            isRevitBusyDispatcher = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, (x, y) => IsRevitBusy = (DateTime.Now - Application.LastTimeWhen).TotalSeconds > 0.5, Dispatcher.CurrentDispatcher);

            CheckIfNewVersionIsAvailable(ver).Forget();

            List.MemberSnooped += () => ListViewItem_MouseLeftButtonUp(null, null);
        }  
        public MainWindow(IList<SnoopableObject> objects) : this()
        {
            PopulateTreeView(objects);
        }


        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Exception.ShowErrorMsg("MainWindow::UnhandledException");
            e.Handled = true;
        }
        private async Task CheckIfNewVersionIsAvailable(Version ver)
        {
            var (isNew, link) = await VersionChecker.Check(ver);
            if (isNew) 
            {
                Title += $" - (a new version is available: {link})";
            }
        }
        private async void SelectorButton_Click(object sender, RoutedEventArgs e)
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
        private void SnoopEvents_Click(object sender, RoutedEventArgs e)
        {           
            ResetTreeItems();
            ResetListItems();
            ResetDatabaseQuery();

            var snoopableObjects = EventMonitor.GetEvents().Select(x => new SnoopableObjectTreeVM(x) { IsExpanded = true }).ToList();
            TreeItems = new(snoopableObjects);            
        }
        private async void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ResetListItems();
            
            if (e.NewValue is SnoopableObjectTreeVM snoopableObjectVM)
            {
                var snoopableMembers = await ExternalExecutor.ExecuteInRevitContextAsync(x => snoopableObjectVM.Object.GetMembers(x).ToList());
                snoopableMembers.ForEach(x => x.SnoopableObjectChanged += () => ReloadButton_Click(null, null));
                List.PopulateListView(snoopableMembers, ListViewFilter);
            }
        }
        private async void ListViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {           
            if (List.ListSelectedItem?.CanBeSnooped == true)
            {
                var snoopableObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x => List.ListSelectedItem.Snooop().ToList());
                var window = new MainWindow(snoopableObjects);
                window.Owner = this;
                window.Show();
            }            
        }
        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {            
            await ExternalExecutor.ExecuteInRevitContextAsync(uiApp => List.ReloadItems());    
        }

        private async void ResetDatabaseFilter_Click(object sender, RoutedEventArgs e)
        {
            await ExternalExecutor.ExecuteInRevitContextAsync(uiApp => ResetDatabaseQuery());    
        }
        private async void ResetItemFilter_Click(object sender, RoutedEventArgs e)
        {
            await ExternalExecutor.ExecuteInRevitContextAsync(uiApp => ResetTreeViewFilter());    
        }

        private async void ResetPropertyFilter_Click(object sender, RoutedEventArgs e)
        {
            await ExternalExecutor.ExecuteInRevitContextAsync(uiApp => ResetListFilter());    
        }




        private async void TryQueryDatabase(string query)
        {            
            ResetTreeItems();
            ResetListItems();

            var snoopableObjects = await ExternalExecutor.ExecuteInRevitContextAsync(uiApp =>
            {
                var document = uiApp?.ActiveUIDocument?.Document;

                if (document == null) return Enumerable.Empty<SnoopableObject>().ToList();

                var result = RevitDatabaseQueryService.Parse(document, query);
                DatabaseQueryToolTip = result.CollectorSyntax;
                QueryVisualization.Update(result.Commands).Forget();
                return result.Collector.ToElements().Select(x => new SnoopableObject(document, x)).ToList();
            });
            PopulateTreeView(snoopableObjects);            
        }        

        
        private void PopulateTreeView(IList<SnoopableObject> objects)
        {
            GroupTreeVM groupTreeVM = new GroupTreeVM("", objects, TreeViewFilter, GroupBy.TypeName);
            groupTreeVM.Expand(true);
            groupTreeVM.SelectFirstDeepestVisibleItem();

            if (groupTreeVM.Items.Count == 1)
            {
                TreeItems = new(new[] { groupTreeVM.Items.First() });
            }
            else
            {
                TreeItems = new(new[] { groupTreeVM });
            }
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
        
        private void FilterTreeView()
        {
            if (TreeItems != null)
            {
                foreach (var item in TreeItems.OfType<GroupTreeVM>())
                {
                    item.Refresh();
                }
            }
        }       
        
        private void ResetListItems()
        {
            List.PopulateListView(System.Array.Empty<SnoopableMember>(), ListViewFilter);
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
            queryVisualizationVM.Update(Enumerable.Empty<RDQCommand>()).Forget();
        }
        private void ResetTreeViewFilter()
        {
            TreeItemsFilterPhrase = "";
        }
        private void ResetListFilter()
        {
            ListItemsFilterPhrase = "";
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
   

        private DispatcherTimer window_SizeChanged_Debouncer;
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            window_SizeChanged_Debouncer = window_SizeChanged_Debouncer.Debounce(TimeSpan.FromSeconds(1), SaveUserSettings);               
        }
        private void SaveUserSettings()
        {
            AppSettings.Default.MainWindowHeight = Height;
            AppSettings.Default.MainWindowWidth = Width;
            AppSettings.Default.FirstColumnWidth = cFirstColumnDefinition.Width.Value;
            AppSettings.Default.Save();
        }

        private void Window_MenuItemSelectTheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                AppSettings.Default.Theme = menuItem.Tag.ToString();
                foreach (ResourceDictionary dict in Resources.MergedDictionaries)
                {
                    if (dict is ThemeResourceDictionary skinDict)
                        skinDict.UpdateSource();
                    //else
                     //   dict.Source = dict.Source;
                }
            }
        }
        private void Window_MenuItemSnoopEvents_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                AppSettings.Default.IsEventMonitorEnabled = menuItem.IsChecked;
            }
        }
        private void TextBox_MenuItem_CopyFilteredElementCollector(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(DatabaseQueryToolTip);
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