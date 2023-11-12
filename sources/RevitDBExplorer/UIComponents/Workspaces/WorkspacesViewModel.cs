using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Properties;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.List.ViewModels;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Workspaces
{
    internal class WorkspacesViewModel : BaseViewModel
    {
        private readonly Action<string> openRDSWithGivenScript;
        private readonly IAmWindowOpener windowOpener;
        private readonly IAmQueryExecutor queryExecutor;
        private readonly ObservableCollection<WorkspaceViewModel> workspaces = new ObservableCollection<WorkspaceViewModel>();
        private WorkspaceViewModel selectedWorkspace;
        private double firstColumnWidth;
        private int tabTitleMaxLength =27;
        private double width;

        public event Action<SelectedItemChangedEventArgs> SelectedItemsChanged;
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
                SelectedItemsChanged?.Invoke(new SelectedItemChangedEventArgs(null, null));
                OnPropertyChanged();
            }
        }
        public int TabTitleMaxLength
        {
            get
            {
                return tabTitleMaxLength;
            }
            set
            {
                tabTitleMaxLength = value;
                OnPropertyChanged();
            }
        }
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;                
                AdjustTabTitleLength();
            }
        }


        public WorkspacesViewModel(IAmWindowOpener windowOpener, IAmQueryExecutor queryExecutor, Action<string> openRDSWithGivenScript)
        {
            this.windowOpener = windowOpener;
            this.queryExecutor = queryExecutor;
            this.openRDSWithGivenScript = openRDSWithGivenScript;
            firstColumnWidth = AppSettings.Default.FirstColumnWidth;                       
        }


        public void Reset()
        {  
            AnnihilateAllWorkspacesAfterThatOne(null);
            SetSelectedWorkspace(null);
            OnPropertyChanged();
        }
        public void OpenWorkspace(SourceOfObjects sourceOfObjects, bool workspaceForEvents = false)
        {
            var workspace = GetFirstAvailableWorkspace();
            workspace.PopulateExplorerTree(sourceOfObjects, workspaceForEvents);
            workspace.IsActive = true;
            SetSelectedWorkspace(workspace);

        }
        public double GetFirstColumnWidth()
        {
            if (SelectedWorkspace != null)
            {
                return SelectedWorkspace.FirstColumnWidth.Value;
            }
            return firstColumnWidth;
        }
        public IEnumerable<object> GetSelectedItems()
        {
            if (SelectedWorkspace != null)
            {
                return SelectedWorkspace.GetSelectedItems();
            }
            return Enumerable.Empty<object>();
        }
        public void CleanUpAtTheEnd()
        {
            foreach (var workspace in Workspaces)
            {
                workspace.UnbindEvents();
            }
        }


        private void SetSelectedWorkspace(WorkspaceViewModel workspaceViewModel)
        {
            selectedWorkspace = workspaceViewModel;
            OnPropertyChanged(nameof(SelectedWorkspace));
            AdjustTabTitleLength();
        }
        private WorkspaceViewModel GetFirstAvailableWorkspace()
        {
            var workspace = Workspaces.Where(x => x.IsActive == false).FirstOrDefault();
            if (workspace == null)
            {
                workspace = CreateNewWorkspace();
                Workspaces.Add(workspace);
            }
            workspace.FirstColumnWidth = new GridLength(GetFirstColumnWidth());
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
                    workspace.Reset();
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
            openNewWindow |= AppSettings.Default.OpenLinksInNewWindow;
            if (openNewWindow)
            {
                windowOpener.Open(sourceOfObjects);
            }
            else
            {
                AnnihilateAllWorkspacesAfterThatOne(sender);
                OpenWorkspace(sourceOfObjects);
            }
        }

        private void Workspace_TreeSelectedItemChanged(TreeSelectedItemChangedEventArgs eventArgs)
        {
            SelectedItemsChanged?.Invoke(new SelectedItemChangedEventArgs(eventArgs.NewOne, null));
        }
        private void Workspace_ListSelectedItemChanged(ListSelectedItemChangedEventArgs eventArgs)
        {
            SelectedItemsChanged?.Invoke(new SelectedItemChangedEventArgs(null, eventArgs.NewOne));
        }

        public void AdjustTabTitleLength()
        {
            if (!double.IsNaN(width))
            {
                var activeWorkspaces = Workspaces.Count(x => x.IsActive == true);
                var availableWidth = Math.Max(width - 222, 200);


                int magicValue = (int)Math.Floor((availableWidth / activeWorkspaces) / 5.55) - 3;
                TabTitleMaxLength = Math.Max(magicValue, 7);
                Workspaces.ForEach(x => x.RefreshTab());
            }
        }
    }

    internal record class SelectedItemChangedEventArgs(TreeItem treeItem, IListItem listItem);
}