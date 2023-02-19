using System;
using System.ComponentModel;
using System.Diagnostics;
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
using RevitDBExplorer.Domain.Selectors;
using RevitDBExplorer.Properties;
using RevitDBExplorer.UIComponents.CommandAndControl;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.QueryVisualization;
using RevitDBExplorer.UIComponents.Scripting;
using RevitDBExplorer.UIComponents.Tree;
using RevitDBExplorer.UIComponents.Tree.Items;
using RevitDBExplorer.WPF;
using RevitDBExplorer.WPF.Controls;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Command;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    internal enum RightView { None, List, CommandAndControl }

    internal partial class MainWindow : Window, INotifyPropertyChanged, IScriptRunner
    {
        private readonly TreeVM treeVM = new();
        private readonly ListVM listVM = new();
        private readonly CommandAndControlVM commandAndControlVM = new();
        private readonly QueryVisualizationVM queryVisualizationVM = new();
        private readonly RDScriptingVM rdscriptingVM;
        private RightView rightView;          
        private string databaseQuery = string.Empty;
        private string databaseQueryToolTip = string.Empty;
        private bool isPopupOpen;        
        private bool isRevitBusy;
        public bool isNewVerAvailable;
        private string newVersionLink;
        private bool isWiderThan800px;
        private readonly DispatcherTimer isRevitBusyDispatcher;
        private readonly IAutocompleteItemProvider databaseQueryAutocompleteItemProvider = new AutocompleteItemProvider();


        public ListVM List => listVM;
        public TreeVM Tree => treeVM;
        public CommandAndControlVM CommandAndControl => commandAndControlVM;
        public QueryVisualizationVM QueryVisualization => queryVisualizationVM;
        public RDScriptingVM Scripting => rdscriptingVM;
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
        public string DatabaseQuery
        {
            get
            {
                return databaseQuery;
            }
            set
            {
                databaseQuery = value;
                if (IsPopupOpen == false)
                {
                    TryQueryDatabase(value);
                }
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
        public bool IsPopupOpen
        {
            get
            {
                return isPopupOpen;
            }
            set
            {
                isPopupOpen = value;
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
        public bool IsNewVerAvailable
        {
            get
            {
                return isNewVerAvailable;
            }
            set
            {
                isNewVerAvailable = value;
                OnPropertyChanged();
            }
        }
        public string NewVersionLink
        {
            get
            {
                return newVersionLink;
            }
            set
            {
                newVersionLink = value;
                OnPropertyChanged();
            }
        }
        public bool IsWiderThan800px
        {
            get
            {
                return isWiderThan800px;
            }
            set
            {
                isWiderThan800px = value;
                OnPropertyChanged();
            }
        }
        public IAutocompleteItemProvider DatabaseQueryAutocompleteItemProvider
        {
            get
            {
                return databaseQueryAutocompleteItemProvider;
            }
        }
        public RelayCommand OpenScriptingWithQueryCommand { get; }
        

        public MainWindow()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            InitializeComponent();
            this.DataContext = this;
            rdscriptingVM = new RDScriptingVM(this);

            var ver = GetType().Assembly.GetName().Version;
            var revit_ver = typeof(Autodesk.Revit.DB.Element).Assembly.GetName().Version;
            Title += $" 20{revit_ver.Major} - {ver.ToGitHubTag()}";

            isRevitBusyDispatcher = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, (x, y) => IsRevitBusy = (DateTime.Now - Application.LastTimeWhen).TotalSeconds > 0.5, Dispatcher.CurrentDispatcher);

            CheckIfNewVersionIsAvailable(ver).Forget();

            List.MemberSnooped += List_MemberSnooped;          
            Tree.SelectedItemChanged += Tree_SelectedItemChanged;
            OpenScriptingWithQueryCommand = new RelayCommand(OpenScriptingWithQuery);
        }  
        public MainWindow(SourceOfObjects sourceOfObjects) : this()
        {
            Tree.PopulateTreeView(sourceOfObjects);
        }


        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Exception.ShowErrorMsg("MainWindow::UnhandledException");
            e.Handled = true;
        }
        private async Task CheckIfNewVersionIsAvailable(Version ver)
        {
            (IsNewVerAvailable, var link) = await VersionChecker.Check(ver);
            if (IsNewVerAvailable) 
            {
                NewVersionLink = link;
            }
        }
        private async void SelectorButton_Click(object sender, RoutedEventArgs e)
        {
            Tree.ClearItems();            
            List.ClearItems();           
            ResetDatabaseQuery();

            var tag = ((Control)sender).Tag as string;
            var selector = (Selector)Enum.Parse(typeof(Selector), tag);
            if (selector == Selector.PickEdge || selector ==  Selector.PickFace)
            {
                //this.WindowState = WindowState.Minimized;
            }
            
            var sourceOfObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x =>
            {
                var source = SelectorFactory.Snoop(selector);
                source.ReadFromTheSource(x);
                return source;
            });

            if (selector == Selector.PickEdge || selector == Selector.PickFace)
            {
                //this.WindowState = WindowState.Normal;
            }       
                
            Tree.PopulateTreeView(sourceOfObjects);            
        }
        private void SnoopEvents_Click(object sender, RoutedEventArgs e)
        {           
            Tree.ClearItems();            
            List.ClearItems();         
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
            if (treeViewItemVM is GroupTreeItem groupTreeItemVM)
            {
                //if (AppSettings.Default.FeatureFlag)
                //{
                //    RightView = RightView.CommandAndControl;
                //    await CommandAndControl.SetInput(groupTreeItemVM);
                //    return;
                //}
            }
            RightView = RightView.None;
        }
        private async void List_MemberSnooped(SnoopableMember member)
        {            
            var sourceOfObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x =>
            {
                var source = member.Snoop();
                source.ReadFromTheSource(x);
                return source;
            });
            var window = new MainWindow(sourceOfObjects);
            window.Owner = this;
            window.Show();                      
        }
        private async void TryQueryDatabase(string query)
        {
            Tree.ClearItems();
            List.ClearItems();            
            
            var rdqResult =  await ExternalExecutor.ExecuteInRevitContextAsync(x => 
            {
                var result = RevitDatabaseQueryService.ParseAndExecute(x.ActiveUIDocument?.Document, query);
                result.SourceOfObjects.ReadFromTheSource(x);
                return result;
            });

            DatabaseQueryToolTip = rdqResult.GeneratedCSharpSyntax;
            QueryVisualization.Update(rdqResult.Commands).Forget();
            Tree.PopulateTreeView(rdqResult.SourceOfObjects);            
        }    
        async Task IScriptRunner.TryExecuteQuery(SourceOfObjects source)
        {
            Tree.ClearItems();
            List.ClearItems();
            ResetDatabaseQuery();

            var sourceOfObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x =>
            {                
                source.ReadFromTheSource(x);
                return source;
            });

            Tree.PopulateTreeView(sourceOfObjects);
        }
     
        private void ResetDatabaseQuery()
        {
            databaseQuery = "";
            OnPropertyChanged(nameof(DatabaseQuery));
            DatabaseQueryToolTip = "";
            queryVisualizationVM.Update(Enumerable.Empty<RDQCommand>()).Forget();
        }


        private void OpenScriptingWithQuery(object parameter)
        {
            Scripting.Open(DatabaseQueryToolTip);
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
            if (e.Key == Key.Delete) 
            {
                //var vkey = KeyInterop.VirtualKeyFromKey(e.Key);
                //Application.RevitWindowHandle.PostKeyMessage(vkey);
            }
        }
   

        private DispatcherTimer window_SizeChanged_Debouncer;        

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            IsWiderThan800px = this.Width > 919;
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

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
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