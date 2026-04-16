using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Properties;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.List.ViewModels;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.Utils;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Workspaces
{
    internal class WorkspacesViewModel : BaseViewModel, IRecipient<OpenWorkspaceCommand>
    {
        private readonly IMessenger iAmMessenger;
        private readonly ObservableCollection<WorkspaceViewModel> workspaces = new ObservableCollection<WorkspaceViewModel>();
        private WorkspaceViewModel selectedWorkspace;
        private double firstColumnWidth;
        private int tabTitleMaxLength =27;
        private double width;

        public event Action<SelectedItemChangedEventArgs> SelectedItemsChanged;
        public ObservableCollection<WorkspaceViewModel> Workspaces => workspaces;
        public IEnumerable<WorkspaceViewModel> ActiveWorkspaces => workspaces.TakeWhile(o=>o.IsActive);
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


        public WorkspacesViewModel(IMessenger iAmMessenger)
        {
            this.iAmMessenger = iAmMessenger;
            iAmMessenger.RegisterAll(this);
         
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
        internal void ActivateNextWorkspace()
        {
            var activeSpaces = ActiveWorkspaces.ToList();
            var index = activeSpaces.IndexOf(SelectedWorkspace);
            if (activeSpaces.Count() - 1 > index)
            {
                SetSelectedWorkspace(activeSpaces[index + 1]);
            }
        }
        internal void ActivatePreviousWorkspace()
        {
            var activeSpaces = ActiveWorkspaces.ToList();
            var index = activeSpaces.IndexOf(SelectedWorkspace);
            if (0 < index)
            {
                SetSelectedWorkspace(activeSpaces[index - 1]);
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
            workspace.FirstColumnWidth = new GridLength(GetFirstColumnWidth());
            return workspace;
        }
        private WorkspaceViewModel CreateNewWorkspace()
        {
            var workspace = new WorkspaceViewModel(iAmMessenger);
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
        public void Receive(OpenWorkspaceCommand cmd)
        {
            bool openNewWindow = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            openNewWindow |= AppSettings.Default.OpenLinksInNewWindow;
            if (openNewWindow)
            {
                iAmMessenger.Send(new OpenWindowCommand(cmd.SourceOfObjects));
            }
            else
            {
                AnnihilateAllWorkspacesAfterThatOne(cmd.Sender as WorkspaceViewModel);
                OpenWorkspace(cmd.SourceOfObjects);
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

    internal sealed record OpenWorkspaceCommand(SourceOfObjects SourceOfObjects) : IHaveSender
    {
        public object Sender { get; set; }
    }
}