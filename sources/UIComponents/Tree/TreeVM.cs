using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Presentation;
using RevitDBExplorer.Domain.RevitDatabaseScripting;
using RevitDBExplorer.UIComponents.Tree.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree
{
    internal class TreeVM : BaseViewModel
    {
        private readonly TreeItemsCommands TreeItemsCommands;
        private ObservableCollection<TreeItem> treeItems = new();
        private TreeItem selectedItem;
        private SourceOfObjects sourceOfObjects;
        private GroupBy groupBy = GroupBy.TypeName;
        private string filterPhrase = string.Empty;
        private bool isExpanded = true;
        private bool treeNotForEvents;

        public event Action<TreeItem> SelectedItemChanged;
        public event Action<IEnumerable<object>> InputForRDSHasChanged;
        public event Action<string> ScriptForRDSHasChanged;

        public RelayCommand SwitchViewCommand { get; }
        public RelayCommand ReloadCommand { get; }
        public RelayCommand CollapseCommand { get; }
        public ObservableCollection<TreeItem> TreeItems
        {
            get
            {
                return treeItems;
            }
            set
            {
                treeItems = value;
                OnPropertyChanged();
            }
        }
        public TreeItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        public string FilterPhrase
        {
            get
            {
                return filterPhrase;
            }
            set
            {
                filterPhrase = value;
                FilterTreeView();
                OnPropertyChanged();
            }
        }
        public bool TreeNotForEvents
        {
            get
            {
                return treeNotForEvents;
            }
            set
            {
                treeNotForEvents = value;
                OnPropertyChanged();
            }
        }


        public TreeVM()
        {
            SwitchViewCommand = new RelayCommand(SwitchView);
            ReloadCommand = new RelayCommand(Reload);
            CollapseCommand = new RelayCommand(Collapse);
            TreeItemsCommands = new TreeItemsCommands(new RelayCommand(UseAsInpputForRDS), new RelayCommand(GenerateUpdateQueryRDS));
        }


        public void ClearItems()
        {
            PopulateTreeView(null);
            FilterPhrase = "";
            isExpanded = true;
        }
        public void PopulateTreeView(SourceOfObjects sourceOfObjects)
        {
            TreeNotForEvents = true;
            FilterPhrase = "";
            this.sourceOfObjects = sourceOfObjects;

            if (sourceOfObjects == null)
            {
                TreeItems = new();
                return;
            }

            GroupTreeItem groupTreeVM = new GroupTreeItem(sourceOfObjects, TreeViewFilter, groupBy, TreeItemsCommands);
            if (isExpanded)
            {
                groupTreeVM.Expand(true);
            }
            else
            {
                groupTreeVM.IsExpanded = true;
            }
            groupTreeVM.SelectFirstDeepestVisibleItem();           

            if (groupTreeVM.Items.Count == 1)
            {
                var firstChild = groupTreeVM.Items[0];
                if ((firstChild is GroupTreeItem group) && (groupTreeVM.Name != null) && false) // todo
                {
                    group.Name = groupTreeVM.Name;
                    group.GroupedBy = GroupBy.Source;
                }
                TreeItems = new(new[] { firstChild });
            }
            else
            {
                TreeItems = new(new[] { groupTreeVM });
            }
        }
        public void PopulateWithEvents(IList<SnoopableObject> snoopableObjects)
        {
            TreeNotForEvents = false;
            var snoopableTreeObjects = snoopableObjects.Select(x => new SnoopableObjectTreeItem(x, TreeItemsCommands) { IsExpanded = true }).ToList();
            TreeItems = new(snoopableTreeObjects);
        }
        private bool TreeViewFilter(object item)
        {
            if (item is SnoopableObjectTreeItem snoopableObjectVM)
            {
                return snoopableObjectVM.Object.Name.IndexOf(filterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return true;
        }
        private void FilterTreeView()
        {
            if (TreeItems != null)
            {
                foreach (var item in TreeItems.OfType<GroupTreeItem>())
                {
                    item.Refresh();
                }
            }
        }

        public void RaiseSelectedItemChanged(TreeItem item)
        {
            SelectedItem = item;
            SelectedItemChanged?.Invoke(item);
        }


        private void SwitchView(object parameter)
        {
            groupBy = groupBy == GroupBy.TypeName ? GroupBy.Category : GroupBy.TypeName;
            PopulateTreeView(sourceOfObjects);
        }
        private async void Reload(object parameter)
        {
           await ExternalExecutor.ExecuteInRevitContextAsync(x => sourceOfObjects.ReadFromTheSource(x));
           PopulateTreeView(sourceOfObjects);
        }
        private void Collapse(object parameter)
        {            
            var first = treeItems.FirstOrDefault();
            if (first == null) return;

            if (isExpanded)
            {
                first.Collapse();
                first.IsExpanded = true;
            }
            else
            {
                if (first is GroupTreeItem { Count : < 666 } group)
                {
                    first.Expand(true, 666);
                }
            }
            isExpanded = !isExpanded;
        }
        private void UseAsInpputForRDS(object parameter)
        {
            if (parameter is TreeItem treeViewItem)
            {
                var objects = GetObjectsForTransfer(treeViewItem);
                InputForRDSHasChanged?.Invoke(objects);
            }
        }
        private void GenerateUpdateQueryRDS(object parameter)
        {
            string text = "";

            if (parameter is SnoopableObjectTreeItem snoopableObjectTreeItem)
            {
                if (snoopableObjectTreeItem.Object.Object is Parameter revitParameter)
                {
                    text = CodeGenerator.GenerateUpdateCommandForParameter(revitParameter);
                }
                else
                {
                    text = CodeGenerator.GenerateUpdateCommandForType(snoopableObjectTreeItem.Object.Object?.GetType());
                }
            }
            if (parameter is GroupTreeItem groupTreeItem)
            {
                text = CodeGenerator.GenerateUpdateCommandForType(typeof(object));

                var pointer = groupTreeItem;
                while (pointer != null) 
                {
                    if (pointer is TypeGroupTreeItem typeGroupTreeItem)
                    {
                        text = CodeGenerator.GenerateUpdateCommandForType(typeGroupTreeItem.GetAllSnoopableObjects().FirstOrDefault()?.Object?.GetType());
                        break;
                    }

                    pointer = pointer.Parent;
                }
            }

            ScriptForRDSHasChanged?.Invoke(text);
        }


        public static IEnumerable<object> GetObjectsForTransfer(TreeItem treeViewItem)
        {
            return treeViewItem.GetAllSnoopableObjects().Where(x => x.Object != null).Select(x => x.Object).ToArray();
        }
    }
}