using System;
using System.Collections.Generic;
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
using RevitDBExplorer.Augmentations.RevitDatabaseVisualization.DrawingVisuals;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Domain.RevitDatabaseScripting;
using RevitDBExplorer.Domain.Selectors;
using RevitDBExplorer.Properties;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.List.ViewModels;
using RevitDBExplorer.UIComponents.QueryEditor;
using RevitDBExplorer.UIComponents.QueryVisualization;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.UIComponents.Workspaces;
using RevitDBExplorer.Utils;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{
    internal partial class MainWindow : Window, IAmWindowOpener, INotifyPropertyChanged
    {
        private readonly QueryEditorViewModel queryEditorVM;
        private readonly QueryVisualizationVM queryVisualizationVM;
        private readonly WorkspacesViewModel workspacesVM;        
        private readonly DispatcherTimer isRevitBusyDispatcher;
        private readonly IRDV3DController rdvController;
        private readonly GlobalKeyboardHook globalKeyboardHook;
        private bool isRevitBusy;
        private bool isNewVerAvailable;
        private string newVersionLink;
        private bool isWiderThan800px;
        private string mouseStatus;
        private string rdqGeneratedCSharpSyntax = "";
        private Autodesk.Revit.DB.XYZ min;
        private Autodesk.Revit.DB.XYZ max;

        public QueryEditorViewModel QueryEditor => queryEditorVM;
        public QueryVisualizationVM QueryVisualization => queryVisualizationVM;
        public WorkspacesViewModel Workspaces => workspacesVM;                                  
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
            Workspaces.OpenWorkspace(sourceOfObjects);  
        }
        public MainWindow()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            queryEditorVM = new QueryEditorViewModel(TryQueryDatabase, GenerateScriptForQueryAndOpenRDS);
            queryVisualizationVM = new QueryVisualizationVM();
            workspacesVM = new WorkspacesViewModel(this, queryEditorVM, OpenRDSWithGivenScript);            
            
            rdvController = RevitDatabaseVisualizationFactory.CreateController();

            InitializeComponent();
            InitializeAsync().Forget();
            this.DataContext = this; 

            Title = WindowTitleGenerator.Get();
            isRevitBusyDispatcher = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, IsRevitBusyDispatcher_Tick, Dispatcher.CurrentDispatcher);

            workspacesVM.SelectedItemsChanged += Workspaces_SelectedItemChanged;

            globalKeyboardHook = new GlobalKeyboardHook();
            globalKeyboardHook.KeyDown += GlobalKeyboardHook_KeyDown;
        }

        
        private async Task InitializeAsync()
        {
            (IsNewVerAvailable, NewVersionLink) = await VersionChecker.CheckIfNewVersionIsAvailable();
        }


        private void IsRevitBusyDispatcher_Tick(object sender, EventArgs e)
        {
            IsRevitBusy = Application.IsRevitBussy();
            (MouseStatus,  min,  max, var isValid) = Application.GetMouseStatus();
            cSelectorButtonScreen.IsEnabled = isValid;
            var v = max - min;
            rdvController.ScaleFactor = v.GetLength();
        }
        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Exception.ShowErrorMsg("MainWindow::UnhandledException");
            e.Handled = true;
        }
       
        private async void SelectorButton_Click(object sender, RoutedEventArgs e)
        {
            Workspaces.Reset();                     
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

            Workspaces.OpenWorkspace(sourceOfObjects);            
        }
        private void SnoopEvents_Click(object sender, RoutedEventArgs e)
        {
            Workspaces.Reset();         
            ResetDatabaseQuery();

            var snoopableObjects = EventMonitor.GetEvents().ToList();
            var source = new SourceOfObjects(snoopableObjects) { Info = new InfoAboutSource("Event Monitor") };
            Workspaces.OpenWorkspace(source, true);            
        }
        private void Workspaces_SelectedItemChanged(SelectedItemChangedEventArgs obj)
        {
            UpdateRDV();
        }       
        void IAmWindowOpener.Open(SourceOfObjects sourceOfObjects)
        {           
            SaveUserSettings();
            var window = new MainWindow(sourceOfObjects);
            window.Owner = this;
            window.Show();           
        }
        private async void TryQueryDatabase(string query)
        {
            Workspaces.Reset();     
            
            var rdqResult =  await ExternalExecutor.ExecuteInRevitContextAsync(x => 
            {
                var result = RevitDatabaseQueryService.ParseAndExecute(x.ActiveUIDocument?.Document, query);
                result.SourceOfObjects.ReadFromTheSource(x);
                return result;
            });

            rdqGeneratedCSharpSyntax = rdqResult.GeneratedCSharpSyntax;
            QueryVisualization.Update(rdqResult.Commands).Forget();
            Workspaces.OpenWorkspace(rdqResult.SourceOfObjects);            
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
                var drawingVisuals = new List<DrawingVisual>();
                foreach (var selectedItem in Workspaces.GetSelectedItems())
                {
                    if (selectedItem is SnoopableObjectTreeItem snoopableObjectTreeItem)
                    {
                        drawingVisuals.AddRange(snoopableObjectTreeItem.Object.GetVisualization());
                    }
                    if (selectedItem is ListItemForMember listItemForMember)
                    {
                        var leftOne = listItemForMember[0];
                        var righttOne = listItemForMember[1];

                        if (leftOne?.ValueViewModel is DefaultPresenter { ValueContainer: { } } presenter)
                        {
                            drawingVisuals.AddRange(presenter.ValueContainer.GetVisualization());                            
                        }
                    }
                }                

                //var line = Autodesk.Revit.DB.Line.CreateBound(min, max);
                //var cd = new CurveDrawingVisual(line, new Autodesk.Revit.DB.Color(255, 0, 0));
                //drawingVisuals.AddRange(new DrawingVisual[] { cd, new CoordinateSystemDrawingVisual(min), new CoordinateSystemDrawingVisual(max) });

                rdvController.AddDrawingVisuals(drawingVisuals);
            }
        }


        private void GenerateScriptForQueryAndOpenRDS()
        {
            var scriptText = new Query_SelectTemplate().Evaluate(rdqGeneratedCSharpSyntax);
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
            globalKeyboardHook.unhook();
            rdvController.Dispose();
            //Application.RevitWindowHandle.BringWindowToFront();
            Dispatcher.UnhandledException -= Dispatcher_UnhandledException;           
            isRevitBusyDispatcher.Tick -= IsRevitBusyDispatcher_Tick;
            Workspaces.SelectedItemsChanged -= Workspaces_SelectedItemChanged;
            Workspaces.CleanUpAtTheEnd();
        }
        private void Window_Closing(object sender, EventArgs e)
        {
            new WindowInteropHelper(this).Owner = IntPtr.Zero;
            SaveUserSettings();
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
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.XButton2 == MouseButtonState.Pressed)
            {
                workspacesVM.ActivateNextWorkspace();
            }

            if (e.XButton1 == MouseButtonState.Pressed)
            {
                workspacesVM.ActivatePreviousWorkspace();
            }
        }
        private void GlobalKeyboardHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == (int)System.Windows.Forms.Keys.F1)
            {
                if (IsActive)
                {
                    e.Handled = true;
                    var selectedItems = Workspaces.GetSelectedItems();
                    var listItemForMember = selectedItems.OfType<ListItemForMember>().FirstOrDefault();
                    var snoopableObjectTreeItem = selectedItems.OfType<SnoopableObjectTreeItem>().FirstOrDefault();
                    if (listItemForMember != null)
                    {
                        CHMService.OpenCHM(listItemForMember[0] ?? listItemForMember[1]);
                        return;
                    }
                    if (snoopableObjectTreeItem != null)
                    {
                        var key = snoopableObjectTreeItem.Object.TypeName;
                        CHMService.OpenCHM(key);
                        return;
                    }
                    
                    CHMService.OpenCHM();
                }
            }
        }


        private DispatcherTimer window_SizeChanged_Debouncer;  
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            IsWiderThan800px = this.Width > 800;
            window_SizeChanged_Debouncer = window_SizeChanged_Debouncer.Debounce(TimeSpan.FromSeconds(4), SaveUserSettings);               
        }
        private void SaveUserSettings()
        {
            AppSettings.Default.MainWindowHeight = Height;
            AppSettings.Default.MainWindowWidth = Width;
            AppSettings.Default.FirstColumnWidth = Workspaces.GetFirstColumnWidth();
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