using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using RevitDBExplorer.Domain;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.UIComponents.Trees.Explorer;
using RevitDBExplorer.UIComponents.Trees.Utility;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Workspaces
{
    internal enum RightView { None, List, CommandAndControl, CompareAndPinToolInfo }

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
        public string ToolTip
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


        public void Reset()
        {
            ExplorerTree.ClearItems();
            List.ClearItems();
        }
        public void PopulateExplorerTree(SourceOfObjects sourceOfObjects, bool workspaceForEvents)
        {
            if (workspaceForEvents)
            {
                ExplorerTree.PopulateWithEvents(sourceOfObjects);
            }
            else
            {
                ExplorerTree.PopulateTreeView(sourceOfObjects);
            }
            Title = "<???>";
            if (sourceOfObjects.Info is not null)
            {
                Title = sourceOfObjects.Info.ShortTitle.Truncate(40);
                ToolTip = sourceOfObjects.Info.ShortTitle;
            }
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