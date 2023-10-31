using System;
using System.ComponentModel;
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
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Domain.RevitDatabaseScripting;
using RevitDBExplorer.Domain.Selectors;
using RevitDBExplorer.Properties;
using RevitDBExplorer.UIComponents.Breadcrumbs;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.List.ViewModels;
using RevitDBExplorer.UIComponents.QueryEditor;
using RevitDBExplorer.UIComponents.QueryVisualization;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.UIComponents.Trees.Explorer;
using RevitDBExplorer.UIComponents.Trees.Utility;
using RevitDBExplorer.Utils;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    internal enum RightView { None, List, CommandAndControl, CompareAndPinToolInfo }

    internal partial class MainWindow : Window, IAmWindowOpener, INotifyPropertyChanged
    {
        private readonly QueryEditorViewModel queryEditorVM;
        private readonly QueryVisualizationVM queryVisualizationVM = new();
        private readonly ExplorerTreeViewModel explorerTreeVM = new();
        private readonly UtilityTreeViewModel utilityTreeVM = new();
        private readonly ListVM listVM; 
        private readonly BreadcrumbsVM breadcrumbsVM;
        private readonly DispatcherTimer isRevitBusyDispatcher;
        private readonly IRDV3DController rdvController;
        private RightView rightView;  
        private bool isRevitBusy;
        private bool isNewVerAvailable;
        private string newVersionLink;
        private bool isWiderThan800px;
        private string mouseStatus;
        private string rdqGeneratedCSharpSyntax = "";


        public QueryEditorViewModel QueryEditor => queryEditorVM;
        public ExplorerTreeViewModel ExplorerTree => explorerTreeVM;
        public UtilityTreeViewModel UtilityTree => utilityTreeVM;
        public ListVM List => listVM;
        public QueryVisualizationVM QueryVisualization => queryVisualizationVM;       
        public BreadcrumbsVM Breadcrumbs => breadcrumbsVM;
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
        public string MouseStatus
        {
            get
            {
                return mouseStatus;
            }
            set
            {
                mouseStatus = value;
                OnPropertyChanged();
            }
        }      
        public bool IsRDVEnabled
        {
            get
            {
                return rdvController.IsEnabled;
            }
            set
            {
                rdvController.IsEnabled = value;
                UpdateRDV();
                OnPropertyChanged();
            }
        }
        

        public MainWindow(SourceOfObjects sourceOfObjects, IntPtr? parentWindowHandle = null) : this()
        {
            if (parentWindowHandle.HasValue)
            {
                new WindowInteropHelper(this).Owner = parentWindowHandle.Value;
            }
            PopulateExplorerTree(sourceOfObjects);  
        }
        public MainWindow()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            queryEditorVM = new QueryEditorViewModel(TryQueryDatabase, GenerateScriptForQueryAndOpenRDS);
            listVM = new ListVM(this, queryEditorVM);
            breadcrumbsVM = new BreadcrumbsVM();

            InitializeComponent();
            InitializeAsync().Forget();
            this.DataContext = this; 

            Title = WindowTitleGenerator.Get();
            isRevitBusyDispatcher = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, IsRevitBusyDispatcher_Tick, Dispatcher.CurrentDispatcher); 

            ExplorerTree.SelectedItemChanged += Tree_SelectedItemChanged;
            ExplorerTree.ScriptWasGenerated += OpenRDSWithGivenScript;
            UtilityTree.SelectedItemChanged += Tree_SelectedItemChanged;
            UtilityTree.ScriptWasGenerated += OpenRDSWithGivenScript;
            List.SelectedItemChanged += List_SelectedItemChanged;
            
            rdvController = RevitDatabaseVisualizationFactory.CreateController();
        }

       

        private async Task InitializeAsync()
        {
            (IsNewVerAvailable, NewVersionLink) = await VersionChecker.CheckIfNewVersionIsAvailable();
        }


        private void IsRevitBusyDispatcher_Tick(object sender, EventArgs e)
        {
            IsRevitBusy = Application.IsRevitBussy();
            MouseStatus = Application.GetMouseStatus();            
        }
        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Exception.ShowErrorMsg("MainWindow::UnhandledException");
            e.Handled = true;
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
                this.WindowState = WindowState.Minimized;
            }
            
            var sourceOfObjects = await ExternalExecutor.ExecuteInRevitContextAsync(x =>
            {
                var source = SelectorFactory.Create(selector);
                source.ReadFromTheSource(x);
                return source;
            });

            if (selector == Selector.PickEdge || selector == Selector.PickFace)
            {
                this.WindowState = WindowState.Normal;
            }

            PopulateExplorerTree(sourceOfObjects);            
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
            if ((!IsActive && IsLoaded) || ignoreEvents)
                return;           

            if (eventArgs.NewOne != null)
            {
                List.ClearItems();

                if (eventArgs.Sender == ExplorerTree)
                {
                    UtilityTree.RemoveSelection();
                }
                if (eventArgs.Sender == UtilityTree)
                {
                    if (ExplorerTree.SelectedItem != null)
                        ExplorerTree.SelectedItem.IsSelected = false;
                }
            }

            if (eventArgs.NewOne is SnoopableObjectTreeItem snoopableObjectTreeItem)
            {
                RightView = RightView.List;
                UpdateRDV();
                await List.PopulateListView(snoopableObjectTreeItem);                
                return;
            }
            rdvController.RemoveAll();
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
        private void List_SelectedItemChanged(ListSelectedItemChangedEventArgs args)
        {
            if (args.NewOne is ListItemForMember listItemForMember)
            {
                var leftOne = listItemForMember[0];
                var righttOne = listItemForMember[1];

                if (leftOne?.ValueViewModel is DefaultPresenter { ValueContainer: { } } presenter)  
                {
                    var v = presenter.ValueContainer.GetVisualization();
                    
                    //return Enumerable.Empty<DrawingVisual>();
                }
            }
        }
        bool ignoreEvents = false;
        void IAmWindowOpener.Open(SourceOfObjects sourceOfObjects)
        {
            ignoreEvents = true;
            var window = new MainWindow(sourceOfObjects);
            window.Owner = this;
            window.Show();     
            ignoreEvents = false;
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

            rdqGeneratedCSharpSyntax = rdqResult.GeneratedCSharpSyntax;
            QueryVisualization.Update(rdqResult.Commands).Forget();
            ExplorerTree.PopulateTreeView(rdqResult.SourceOfObjects);            
        }    
        

        private void PopulateExplorerTree(SourceOfObjects sourceOfObjects)
        {
            ExplorerTree.PopulateTreeView(sourceOfObjects);
            Breadcrumbs.Set(sourceOfObjects.Title);
        }
        private void ResetDatabaseQuery()
        {
            QueryEditor.ResetDatabaseQuery();
            rdqGeneratedCSharpSyntax = "";
            QueryVisualization.Reset().Forget();
        }
        

        private void UpdateRDV()
        {
            rdvController.RemoveAll();
            if (rdvController.IsEnabled)
            {
                var snoopableObjectTreeItem = ExplorerTree.SelectedItem as SnoopableObjectTreeItem;
                snoopableObjectTreeItem ??= UtilityTree.SelectedItem as SnoopableObjectTreeItem;
                if (snoopableObjectTreeItem != null)
                {
                    rdvController.AddDrawingVisuals(snoopableObjectTreeItem.Object.GetVisualization());                  
                }
            }
        }


        private void GenerateScriptForQueryAndOpenRDS()
        {
            var scriptText = CodeGenerator.GenerateQueryFor(rdqGeneratedCSharpSyntax);
            OpenRDSWithGivenScript(scriptText);
        }
        private void OpenRDSWithGivenScript(string scriptText)
        {               
            OpenRDS();
            Application.RDSController.SetText(scriptText);
        }
        private void RDSButton_Click(object sender, RoutedEventArgs e)
        {
            OpenRDS();
        }
        private void OpenRDS()
        {
            Application.RDSController.Open(this.Left, this.Top + this.ActualHeight);
        }        

               
        private void Window_Closed(object sender, EventArgs e)
        {
            rdvController.Dispose();
            //Application.RevitWindowHandle.BringWindowToFront();
            Dispatcher.UnhandledException -= Dispatcher_UnhandledException;           
            isRevitBusyDispatcher.Tick -= IsRevitBusyDispatcher_Tick;
            ExplorerTree.SelectedItemChanged -= Tree_SelectedItemChanged;
            ExplorerTree.ScriptWasGenerated -= OpenRDSWithGivenScript;
            UtilityTree.SelectedItemChanged -= Tree_SelectedItemChanged;
            UtilityTree.ScriptWasGenerated -= OpenRDSWithGivenScript;        
            List.SelectedItemChanged -= List_SelectedItemChanged;
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
            window_SizeChanged_Debouncer = window_SizeChanged_Debouncer.Debounce(TimeSpan.FromSeconds(4), SaveUserSettings);               
        }
        private void SaveUserSettings()
        {
            AppSettings.Default.MainWindowHeight = Height;
            AppSettings.Default.MainWindowWidth = Width;
            AppSettings.Default.FirstColumnWidth = cFirstColumnDefinition.Width.Value;
            AppSettings.Default.Save();
        }
        private void ConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ConfigWindow();
            window.Owner = this;
            window.ShowDialog();
            ThemeResourceDictionary.Update(Resources);
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