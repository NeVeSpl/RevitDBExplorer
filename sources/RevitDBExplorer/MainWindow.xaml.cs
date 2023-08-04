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
using System.Xml.Linq;
using RevitDBExplorer.Augmentations;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion;
using RevitDBExplorer.Domain.RevitDatabaseScripting;
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

    internal partial class MainWindow : Window, INotifyPropertyChanged
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
        private readonly IBoundingBoxVisualizer boundingBoxVisualizer;


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
            InitializeComponent();
            this.DataContext = this;
            rdscriptingVM = new RDScriptingVM();

            var ver = GetType().Assembly.GetName().Version;
            var revit_ver = typeof(Autodesk.Revit.DB.Element).Assembly.GetName().Version;
            Title += $" 20{revit_ver.Major} - {ver.ToGitHubTag()}";

            isRevitBusyDispatcher = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, (x, y) => IsRevitBusy = Application.IsRevitBussy(), Dispatcher.CurrentDispatcher);

            CheckIfNewVersionIsAvailable(ver).Forget();

            List.MemberSnooped += List_MemberSnooped;          
            Tree.SelectedItemChanged += Tree_SelectedItemChanged;
            Tree.InputForRDSHasChanged += RDSSetInput;
            Tree.ScriptForRDSHasChanged += RDSOpenWithCommand;
            OpenScriptingWithQueryCommand = new RelayCommand(RDSOpenWithQuery);
            SaveQueryAsFavoriteCommand = new RelayCommand(SaveQueryAsFavorite, x => !string.IsNullOrEmpty(DatabaseQuery) );
            boundingBoxVisualizer = BoundingBoxVisualizerFactory.GetInstance();
        }  
        public MainWindow(SourceOfObjects sourceOfObjects, IntPtr? parentWindowHandle = null) : this()
        {
            if (parentWindowHandle.HasValue)
            {
                new WindowInteropHelper(this).Owner = parentWindowHandle.Value;
            }
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
                var source = SelectorFactory.Create(selector);
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

            var snoopableObjects = EventMonitor.GetEvents().ToList();
            Tree.PopulateWithEvents(snoopableObjects);            
        }
        private async void Tree_SelectedItemChanged(SelectedItemChangedEventArgs eventArgs)
        {
            List.ClearItems();

            if (eventArgs.NewOne is SnoopableObjectTreeItem snoopableObjectVM)
            {
                RightView = RightView.List;
                var snoopableMembers = await ExternalExecutor.ExecuteInRevitContextAsync(x => snoopableObjectVM.Object.GetMembers(x).ToList());            
                List.PopulateListView(snoopableMembers);
                boundingBoxVisualizer.Show(snoopableObjectVM.Object.Object as Autodesk.Revit.DB.Element);
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
         
        private void ResetDatabaseQuery()
        {
            databaseQuery = "";
            OnPropertyChanged(nameof(DatabaseQuery));
            DatabaseQueryToolTip = "";
            queryVisualizationVM.Update(Enumerable.Empty<RDQCommand>()).Forget();
        }

        private void RDSSetInput(IEnumerable<object> objects)
        {
            OpenRDS();
            Application.RDSController.SetInput(objects);
        }
        private void RDSOpenWithQuery(object parameter)
        {
            var scriptText = CodeGenerator.GenerateQueryFor(DatabaseQueryToolTip);
            //Scripting.Open();           
            OpenRDS();
            Application.RDSController.SetText(scriptText);
        }        
        private void RDSOpenWithCommand(string scriptText)
        {
            //Scripting.Open();           
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