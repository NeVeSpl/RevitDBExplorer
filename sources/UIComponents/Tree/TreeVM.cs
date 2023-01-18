using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Presentation;
using RevitDBExplorer.UIComponents.Tree.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree
{
    internal class TreeVM : BaseViewModel
    {
        private ObservableCollection<TreeItem> treeItems = new();
        private string treeItemsFilterPhrase = string.Empty;
        private TreeItem selectedItem;

        public event Action<TreeItem> SelectedItemChanged;

        public SelectInRevitCommand SelectInRevit { get; } = SelectInRevitCommand.Instance;
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


        public void ClearItems()
        {
            PopulateTreeView(new());
        }
        public void PopulateTreeView(ResultOfSnooping resultOfSnooping)
        {
            treeItemsFilterPhrase = "";

            GroupTreeItem groupTreeVM = new GroupTreeItem(resultOfSnooping, TreeViewFilter, GroupBy.TypeName);
            groupTreeVM.Expand(true);
            groupTreeVM.SelectFirstDeepestVisibleItem();

            if (groupTreeVM.Items.Count == 1)
            {
                TreeItems = new(new[] { groupTreeVM.Items.First() });
            }
            else
            {
                TreeItems = new(new[] { groupTreeVM });
            }
        }
        public void PopulateWithEvents(IList<SnoopableObjectTreeItem> snoopableObjects)
        {
            TreeItems = new(snoopableObjects);
        }
        private bool TreeViewFilter(object item)
        {
            if (item is SnoopableObjectTreeItem snoopableObjectVM)
            {
                return snoopableObjectVM.Object.Name.IndexOf(treeItemsFilterPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return true;
        }
        public void FilterTreeView(string treeItemsFilterPhrase)
        {
            if (string.Equals(treeItemsFilterPhrase, this.treeItemsFilterPhrase)) return;

            this.treeItemsFilterPhrase = treeItemsFilterPhrase;
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
    }
}