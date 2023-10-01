using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using RevitDBExplorer.Augmentations;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion;
using RevitDBExplorer.Domain.RevitDatabaseScripting;
using RevitDBExplorer.Domain.Selectors;
using RevitDBExplorer.Properties;
using RevitDBExplorer.UIComponents.CommandAndControl;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.QueryVisualization;
using RevitDBExplorer.UIComponents.Scripting;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.UIComponents.Trees.Explorer;
using RevitDBExplorer.UIComponents.Trees.Utility;
using RevitDBExplorer.WPF;
using RevitDBExplorer.WPF.Controls;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Command;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    internal enum RightView { None, List, CommandAndControl, CompareAndPinToolInfo }

    internal partial class MainWindow : Window, IAmWindowOpener, INotifyPropertyChanged
    {
        private readonly ExplorerTreeViewModel explorerTreeViewModel = new();
        private readonly UtilityTreeViewModel utilityTreeViewModel = new();
        private readonly ListVM listVM;
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
        private readonly IBoundingBoxVisualizer boundingBoxVisualizer;


        public ExplorerTreeViewModel ExplorerTree => explorerTreeViewModel;
        public UtilityTreeViewModel UtilityTree => utilityTreeViewModel;
        public ListVM List => listVM;        
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
        public bool IsBoundingBoxVisualizerEnabled
        {
            get
            {
                return boundingBoxVisualizer.IsEnabled;
            }
            set
            {
                boundingBoxVisualizer.IsEnabled = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand OpenScriptingWithQueryCommand { get; }
        public RelayCommand SaveQueryAsFavoriteCommand { get; }

        public MainWindow()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            listVM = new ListVM(this);

            InitializeComponent();
            this.DataContext = this;
            rdscriptingVM = new RDScriptingVM();

            var ver = GetType().Assembly.GetName().Version;
            var revit_ver = typeof(Autodesk.Revit.DB.Element).Assembly.GetName().Version;
            Title += $" 20{revit_ver.Major} - {ver.ToGitHubTag()}";

            isRevitBusyDispatcher = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, IsRevitBusyDispatcher_Tick, Dispatcher.CurrentDispatcher);           
            CheckIfNewVersionIsAvailable(ver).Forget();

            ExplorerTree.SelectedItemChanged += Tree_SelectedItemChanged;
            ExplorerTree.ScriptForRDSHasChanged += RDSOpenWithCommand;
            UtilityTree.SelectedItemChanged += Tree_SelectedItemChanged;
            UtilityTree.ScriptForRDSHasChanged += RDSOpenWithCommand;
            OpenScriptingWithQueryCommand = new RelayCommand(RDSOpenWithQuery);
            SaveQueryAsFavoriteCommand = new RelayCommand(SaveQueryAsFavorite, x => !string.IsNullOrEmpty(DatabaseQuery) );
            boundingBoxVisualizer = BoundingBoxVisualizerFactory.GetInstance();
        }


        private void IsRevitBusyDispatcher_Tick(object sender, EventArgs e)
        {
            IsRevitBusy = Application.IsRevitBussy();
        }
        public MainWindow(SourceOfObjects sourceOfObjects, IntPtr? parentWindowHandle = null) : this()
        {
            if (parentWindowHandle.HasValue)
            {
                new WindowInteropHelper(this).Owner = parentWindowHandle.Value;
            }
            ExplorerTree.PopulateTreeView(sourceOfObjects);
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
            ExplorerTree.ClearItems();            
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
                var source = SelectorFactory.Create(selector);
                source.ReadFromTheSource(x);
                return source;
            });

            if (selector == Selector.PickEdge || selector == Selector.PickFace)
            {
                //this.WindowState = WindowState.Normal;
            }

            ExplorerTree.PopulateTreeView(sourceOfObjects);            
        }
        private void SnoopEvents_Click(object sender, RoutedEventArgs e)
        {
            ExplorerTree.ClearItems();            
            List.ClearItems();         
            ResetDatabaseQuery();

            var snoopableObjects = EventMonitor.GetEvents().ToList();
            ExplorerTree.PopulateWithEvents(snoopableObjects);            
        }
        private async void Tree_SelectedItemChanged(SelectedItemChangedEventArgs eventArgs)
        {
            List.ClearItems();

            if (eventArgs.Sender == ExplorerTree)
            {
                if (UtilityTree.SelectedItem != null)
                    UtilityTree.SelectedItem.IsSelected = false;
            }
            if (eventArgs.Sender == UtilityTree)
            {
                if (ExplorerTree.SelectedItem != null)
                    ExplorerTree.SelectedItem.IsSelected = false;
            }

            if (eventArgs.NewOne is SnoopableObjectTreeItem snoopableObjectTreeItem)
            {
                RightView = RightView.List;               
                await List.PopulateListView(snoopableObjectTreeItem);
                boundingBoxVisualizer.Show(snoopableObjectTreeItem.Object.Object as Autodesk.Revit.DB.Element);
                return;
            }
            boundingBoxVisualizer.HideAll();
            if (eventArgs.NewOne is GroupTreeItem groupTreeItemVM)
            {
                //if (AppSettings.Default.FeatureFlag)
                //{
                //    RightView = RightView.CommandAndControl;
                //    await CommandAndControl.SetInput(groupTreeItemVM);
                //    return;
                //}
            }
            if (eventArgs.NewOne is UtilityGroupTreeItem utilityGroupTreeItem)
            {                
                var wasSuccessful = await List.PopulateListView(utilityGroupTreeItem);
                if (wasSuccessful)
                {
                    RightView = RightView.List;
                }
                else
                {
                    RightView = RightView.CompareAndPinToolInfo;
                }
                return;                   
            }
            RightView = RightView.None;
        }
        void IAmWindowOpener.Open(SourceOfObjects sourceOfObjects)
        {
            if (UtilityTree.SelectedItem != null)
                UtilityTree.SelectedItem.IsSelected = false;
            var window = new MainWindow(sourceOfObjects);
            window.Owner = this;
            window.Show();                      
        }
        private async void TryQueryDatabase(string query)
        {
            ExplorerTree.ClearItems();
            List.ClearItems();            
            
            var rdqResult =  await ExternalExecutor.ExecuteInRevitContextAsync(x => 
            {
                var result = RevitDatabaseQueryService.ParseAndExecute(x.ActiveUIDocument?.Document, query);
                result.SourceOfObjects.ReadFromTheSource(x);
                return result;
            });

            DatabaseQueryToolTip = rdqResult.GeneratedCSharpSyntax;
            QueryVisualization.Update(rdqResult.Commands).Forget();
            ExplorerTree.PopulateTreeView(rdqResult.SourceOfObjects);            
        }    
         
        private void ResetDatabaseQuery()
        {
            databaseQuery = "";
            OnPropertyChanged(nameof(DatabaseQuery));
            DatabaseQueryToolTip = "";
            queryVisualizationVM.Update(Enumerable.Empty<RDQCommand>()).Forget();
        }

     
        private void RDSOpenWithQuery(object parameter)
        {
            var scriptText = CodeGenerator.GenerateQueryFor(DatabaseQueryToolTip);                 
            OpenRDS();
            Application.RDSController.SetText(scriptText);
        }        
        private void RDSOpenWithCommand(string scriptText)
        {               
            OpenRDS();
            Application.RDSController.SetText(scriptText);
        }
        private void SaveQueryAsFavorite(object parameter)
        {
            FavoritesManager.Add(DatabaseQuery);
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
            boundingBoxVisualizer.Dispose();
            //Application.RevitWindowHandle.BringWindowToFront();
            Dispatcher.UnhandledException -= Dispatcher_UnhandledException;           
            isRevitBusyDispatcher.Tick -= IsRevitBusyDispatcher_Tick;
            ExplorerTree.SelectedItemChanged -= Tree_SelectedItemChanged;
            ExplorerTree.ScriptForRDSHasChanged -= RDSOpenWithCommand;
            UtilityTree.SelectedItemChanged -= Tree_SelectedItemChanged;
            UtilityTree.ScriptForRDSHasChanged -= RDSOpenWithCommand;
        }
        private void Window_Closing(object sender, EventArgs e)
        {
            new WindowInteropHelper(this).Owner = IntPtr.Zero;
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
            IsWiderThan800px = this.Width > 848;
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

        private void RDS_Click(object sender, RoutedEventArgs e)
        {
            OpenRDS();         
        }
        private void OpenRDS()
        {
            Application.RDSController.Open(this.Left, this.Top + this.ActualHeight);
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