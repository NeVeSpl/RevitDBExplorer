using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Properties;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.List.ViewModels;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.UIComponents.Trees.Explorer;
using RevitDBExplorer.UIComponents.Trees.Utility;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Workspaces
{
    internal enum RightView { None, List, CommandAndControl, CompareAndPinToolInfo }

    internal class WorkspacesViewModel : BaseViewModel
    {
        private readonly Action<string> openRDSWithGivenScript;
        private readonly IAmWindowOpener windowOpener;
        private readonly IAmQueryExecutor queryExecutor;
        private readonly ObservableCollection<WorkspaceViewModel> workspaces = new ObservableCollection<WorkspaceViewModel>();
        private WorkspaceViewModel selectedWorkspace;

        public event Action<SelectedItemChangedEventArgs> SelectedItemChanged;
        public ObservableCollection<WorkspaceViewModel> Workspaces => workspaces;
        public WorkspaceViewModel SelectedWorkspace
        {
            get
            {
                return selectedWorkspace;
            }
            set
            {
                selectedWorkspace = value;
                OnPropertyChanged();
            }
        }

        public WorkspacesViewModel(IAmWindowOpener windowOpener, IAmQueryExecutor queryExecutor, Action<string> openRDSWithGivenScript)
        {
            this.windowOpener = windowOpener;
            this.queryExecutor = queryExecutor;
            this.openRDSWithGivenScript = openRDSWithGivenScript;   
            Workspaces.Add(CreateNewWorkspace());            
        }


        public void Reset()
        {  
            AnnihilateAllWorkspacesAfterThatOne(null);
            SelectedWorkspace = null;
        }
        public void PopulateExplorerTree(SourceOfObjects sourceOfObjects)
        {
            var workspace = GetFirstAvailableWorkspace();
            workspace.PopulateExplorerTree(sourceOfObjects);
            workspace.IsActive = true;
            workspace.Title = sourceOfObjects.Title ?? "??";
            SelectedWorkspace = workspace;

        }
        public void PopulateExplorerTreeWithEvents(IList<SnoopableObject> snoopableObjects)
        {
            var workspace = GetFirstAvailableWorkspace();
            workspace.PopulateExplorerTreeWithEvents(snoopableObjects);
            workspace.IsActive = true;
            SelectedWorkspace = workspace;
        }
        public double GetFirstColumnWidth()
        {
            return Workspaces.First().FirstColumnWidth.Value;
        }
        public IEnumerable<object> GetSelectedItems()
        {
            if ((SelectedWorkspace != null) && (SelectedWorkspace.IsActive))
            {
                return SelectedWorkspace.GetSelectedItems();
            }
            return Enumerable.Empty<object>();
        }
        public void UnbindEvents()
        {
            foreach (var workspace in Workspaces)
            {
                workspace.UnbindEvents();
            }
        }


        private WorkspaceViewModel GetFirstAvailableWorkspace()
        {
            var workspace = Workspaces.Where(x => x.IsActive == false).FirstOrDefault();
            if (workspace == null)
            {
                workspace = CreateNewWorkspace();
                Workspaces.Add(workspace);
            }
            return workspace;
        }
        private WorkspaceViewModel CreateNewWorkspace()
        {
            var workspace = new WorkspaceViewModel(OpenLink, queryExecutor, openRDSWithGivenScript);
            workspace.IsActive = false;
            workspace.ListSelectedItemChanged += Workspace_ListSelectedItemChanged;
            workspace.TreeSelectedItemChanged += Workspace_TreeSelectedItemChanged;  
            return workspace;
        }
        private void AnnihilateAllWorkspacesAfterThatOne(WorkspaceViewModel target)
        {
            bool killThemAll = target == null ? true : false;
            foreach (var workspace in Workspaces) 
            {
                if (killThemAll)
                {
                    workspace.IsActive = false;
                    workspace.ClearItems();
                }
                if (workspace == target)
                {
                    killThemAll = true;
                }
            }
        }
        private void OpenLink(WorkspaceViewModel sender, SourceOfObjects sourceOfObjects)
        {
            bool openNewWindow = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            if (true) //(openNewWindow)
            {
                windowOpener.Open(sourceOfObjects);
            }
            else
            {
                AnnihilateAllWorkspacesAfterThatOne(sender);
                PopulateExplorerTree(sourceOfObjects);
            }
        }

        private void Workspace_TreeSelectedItemChanged(TreeSelectedItemChangedEventArgs eventArgs)
        {
            SelectedItemChanged?.Invoke(new SelectedItemChangedEventArgs(eventArgs.NewOne, null));
        }
        private void Workspace_ListSelectedItemChanged(ListSelectedItemChangedEventArgs eventArgs)
        {
            SelectedItemChanged?.Invoke(new SelectedItemChangedEventArgs(null, eventArgs.NewOne));
        }
    }

    internal record class SelectedItemChangedEventArgs(TreeItem treeItem, IListItem listItem);


    internal class WorkspaceViewModel : BaseViewModel, IAmWindowOpener
    {
        private readonly Action<string> openRDSWithGivenScript;
        private readonly Action<WorkspaceViewModel, SourceOfObjects> openLink;
        private readonly ExplorerTreeViewModel explorerTreeVM = new();
        private readonly UtilityTreeViewModel utilityTreeVM = new();
        private readonly ListVM listVM;
        private RightView rightView;
        private bool isActive;        
        private string title;
        private GridLength firstColumnWidth;
        

        public event Action<TreeSelectedItemChangedEventArgs> TreeSelectedItemChanged;
        public event Action<ListSelectedItemChangedEventArgs> ListSelectedItemChanged;
        public ExplorerTreeViewModel ExplorerTree => explorerTreeVM;
        public UtilityTreeViewModel UtilityTree => utilityTreeVM;
        public ListVM List => listVM;
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
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
        public GridLength FirstColumnWidth
        {
            get
            {
                return firstColumnWidth;
            }
            set
            {
                firstColumnWidth = value;
                OnPropertyChanged();
            }
        }
        

        public WorkspaceViewModel(Action<WorkspaceViewModel, SourceOfObjects> openLink, IAmQueryExecutor queryExecutor, Action<string> openRDSWithGivenScript)
        {
            FirstColumnWidth = new GridLength(AppSettings.Default.FirstColumnWidth);
            this.openLink = openLink;
            this.openRDSWithGivenScript = openRDSWithGivenScript;
            listVM = new ListVM(this, queryExecutor);
            ExplorerTree.SelectedItemChanged += Tree_SelectedItemChanged;
            UtilityTree.SelectedItemChanged += Tree_SelectedItemChanged;
            List.SelectedItemChanged += List_SelectedItemChanged;
            ExplorerTree.ScriptWasGenerated += OpenRDSWithGivenScript;
            UtilityTree.ScriptWasGenerated += OpenRDSWithGivenScript;            
        }


        private async void Tree_SelectedItemChanged(TreeSelectedItemChangedEventArgs eventArgs)
        {
            if (eventArgs.NewOne != null)
            {
                List.ClearItems();

                if (eventArgs.Sender == ExplorerTree)
                {
                    UtilityTree?.RemoveSelection();
                }
                if (eventArgs.Sender == UtilityTree)
                {
                    if (ExplorerTree.SelectedItem != null)
                        ExplorerTree.SelectedItem.IsSelected = false;
                }
            }

            var chosenView = RightView.None;

            if (eventArgs.NewOne is SnoopableObjectTreeItem snoopableObjectTreeItem)
            {
                chosenView = RightView.List;
                List.PopulateListView(snoopableObjectTreeItem).Forget();               
            }

            if (eventArgs.NewOne is UtilityGroupTreeItem utilityGroupTreeItem)
            {
                var wasSuccessful = await List.PopulateListView(utilityGroupTreeItem);
                if (wasSuccessful)
                {
                    chosenView = RightView.List;
                }
                else
                {
                    chosenView = RightView.CompareAndPinToolInfo;
                }                
            }

            RightView = chosenView;

            if (IsActive)
            {
                TreeSelectedItemChanged?.Invoke(eventArgs);
            }
        }
        private void List_SelectedItemChanged(ListSelectedItemChangedEventArgs eventArgs)
        {
            if (IsActive)
            {
                ListSelectedItemChanged?.Invoke(eventArgs);
            }
        }        
        private void OpenRDSWithGivenScript(string scriptText)
        {
            openRDSWithGivenScript(scriptText);
        }


        public void ClearItems()
        {
            ExplorerTree.ClearItems();
            List.ClearItems();
        }
        public void PopulateExplorerTree(SourceOfObjects sourceOfObjects)
        {
            ExplorerTree.PopulateTreeView(sourceOfObjects);
        }
        public void PopulateExplorerTreeWithEvents(IList<SnoopableObject> snoopableObjects)
        {
            ExplorerTree.PopulateWithEvents(snoopableObjects);
        }
        public IEnumerable<object> GetSelectedItems()
        {
            if (List.ListSelectedItem != null)
            {
                yield return List.ListSelectedItem;
            }
            if (ExplorerTree.SelectedItem != null)
            {
                yield return ExplorerTree.SelectedItem;
            }
            if (UtilityTree?.SelectedItem != null)
            {
                yield return UtilityTree.SelectedItem;
            }
        }
        public void UnbindEvents()
        {
            ExplorerTree.SelectedItemChanged -= Tree_SelectedItemChanged;
            UtilityTree.SelectedItemChanged -= Tree_SelectedItemChanged;
            List.SelectedItemChanged -= List_SelectedItemChanged;
            ExplorerTree.ScriptWasGenerated -= OpenRDSWithGivenScript;
            UtilityTree.ScriptWasGenerated -= OpenRDSWithGivenScript;
        }

        void IAmWindowOpener.Open(SourceOfObjects sourceOfObjects)
        {
            openLink(this, sourceOfObjects);
        }
    }
}