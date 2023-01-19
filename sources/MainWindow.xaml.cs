using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Properties;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.QueryVisualization;
using RevitDBExplorer.UIComponents.Tree;
using RevitDBExplorer.UIComponents.Tree.Items;
using RevitDBExplorer.WPF;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Command;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    internal enum RightView { None, List }

    internal partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly TreeVM treeVM = new();
        private readonly ListVM listVM = new();
        private readonly QueryVisualizationVM queryVisualizationVM = new();
        private RightView rightView;
        private string rightFilterPhrase = string.Empty;
        private string leftFilterPhrase = string.Empty;
        private string databaseQuery = string.Empty;
        private string databaseQueryToolTip = string.Empty;
        private bool isRevitBusy;
        private readonly DispatcherTimer isRevitBusyDispatcher;

            
        public ListVM List => listVM;
        public TreeVM Tree => treeVM;
        public RightView RightView
        {
            get
            {
                return rightView;
            }
            set
            {
                rightView = value;
                OnPropertyChanged();
            }
        }
        public QueryVisualizationVM QueryVisualization => queryVisualizationVM;         
        public string RightFilterPhrase
        {
            get
            {
                return rightFilterPhrase;
            }
            set
            {
                rightFilterPhrase = value;
                List.FilterListView(value);                
                OnPropertyChanged();
            }
        }
        public string LeftFilterPhrase
        {
            get
            {
                return leftFilterPhrase;
            }
            set
            {
                leftFilterPhrase = value;
                Tree.FilterTreeView(value);
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

            List.MemberSnooped += List_MemberSnooped;
            List.MemberValueHasChanged += () => ReloadButton_Click(null, null);
            Tree.SelectedItemChanged += Tree_SelectedItemChanged;
        }  
        public MainWindow(ResultOfSnooping resultOfSnooping) : this()
        {
            Tree.PopulateTreeView(resultOfSnooping);
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
            Tree.ClearItems();            
            List.ClearItems();
            LeftFilterPhrase = "";
            ResetDatabaseQuery();

            var tag = ((Control)sender).Tag as string;
            var selector = (Selector)Enum.Parse(typeof(Selector), tag);
            if (selector == Selector.PickEdge || selector ==  Selector.PickFace)
            {
                //this.WindowState = WindowState.Minimized;
            }
            var resultOfSnooping = await ExternalExecutor.ExecuteInRevitContextAsync(x => SelectorExecutor.Snoop(x, selector));
            if (selector == Selector.PickEdge || selector == Selector.PickFace)
            {
                //this.WindowState = WindowState.Normal;
            }       
                
            Tree.PopulateTreeView(resultOfSnooping);            
        }
        private void SnoopEvents_Click(object sender, RoutedEventArgs e)
        {           
            Tree.ClearItems();            
            List.ClearItems();
            LeftFilterPhrase = "";
            ResetDatabaseQuery();

            var snoopableObjects = EventMonitor.GetEvents().Select(x => new SnoopableObjectTreeItem(x) { IsExpanded = true }).ToList();
            Tree.PopulateWithEvents(snoopableObjects);            
        }
        private async void Tree_SelectedItemChanged(TreeItem treeViewItemVM)
        {
            List.ClearItems();

            if (treeViewItemVM is SnoopableObjectTreeItem snoopableObjectVM)
            {
                RightView = RightView.List;
                var snoopableMembers = await ExternalExecutor.ExecuteInRevitContextAsync(x => snoopableObjectVM.Object.GetMembers(x).ToList());            
                List.PopulateListView(snoopableMembers);
                return;
            }
            RightView = RightView.None;
        }
        private async void List_MemberSnooped(SnoopableMember member)
        { 
            var resultOfSnooping = await ExternalExecutor.ExecuteInRevitContextAsync(x => member.Snooop());
            var window = new MainWindow(resultOfSnooping);
            window.Owner = this;
            window.Show();                      
        }
        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {            
            await ExternalExecutor.ExecuteInRevitContextAsync(uiApp => List.ReloadItems());    
        }
        private async void TryQueryDatabase(string query)
        {
            Tree.ClearItems();
            List.ClearItems();
            LeftFilterPhrase = "";            

            var snoopableObjects = await ExternalExecutor.ExecuteInRevitContextAsync(uiApp =>
            {
                var document = uiApp?.ActiveUIDocument?.Document;

                if (document == null) return new ResultOfSnooping();

                var result = RevitDatabaseQueryService.Parse(document, query);
                DatabaseQueryToolTip = result.CollectorSyntax;
                QueryVisualization.Update(result.Commands).Forget();
                var snoopableObjects = result.Collector.ToElements().Select(x => new SnoopableObject(document, x));
                return new ResultOfSnooping(snoopableObjects.ToArray());
            });
            Tree.PopulateTreeView(snoopableObjects);            
        }                                    
     
        private void ResetDatabaseQuery()
        {
            databaseQuery = "";
            OnPropertyChanged(nameof(DatabaseQuery));
            DatabaseQueryToolTip = "";
            queryVisualizationVM.Update(Enumerable.Empty<RDQCommand>()).Forget();
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
        private void Window_MenuItem_ConfigurationClick(object sender, RoutedEventArgs e)
        {
            var window = new ConfigWindow();
            window.Owner = this;
            window.ShowDialog();
            foreach (ResourceDictionary dict in Resources.MergedDictionaries)
            {
                if (dict is ThemeResourceDictionary skinDict)
                    skinDict.UpdateSource();                
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