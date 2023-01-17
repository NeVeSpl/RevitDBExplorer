using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Presentation;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree
{
    internal class TreeVM : BaseViewModel
    {
        private ObservableCollection<TreeViewItemVM> treeItems = new();
        private string treeItemsFilterPhrase = string.Empty;
        private TreeViewItemVM selectedItem;

        public event Action<TreeViewItemVM> SelectedItemChanged;

        public SelectInRevitCommand SelectInRevit { get; } = SelectInRevitCommand.Instance;
        public ObservableCollection<TreeViewItemVM> TreeItems
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
        public TreeViewItemVM SelectedItem
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
            PopulateTreeView(System.Array.Empty<SnoopableObject>());
        }
        public void PopulateTreeView(IList<SnoopableObject> objects)
        {
            treeItemsFilterPhrase = "";

            GroupTreeVM groupTreeVM = new GroupTreeVM("", objects, TreeViewFilter, GroupBy.TypeName);
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
        public void PopulateWithEvents(IList<SnoopableObjectTreeVM> snoopableObjects)
        {
            TreeItems = new(snoopableObjects);
        }
        private bool TreeViewFilter(object item)
        {
            if (item is SnoopableObjectTreeVM snoopableObjectVM)
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
                foreach (var item in TreeItems.OfType<GroupTreeVM>())
                {
                    item.Refresh();
                }
            }
        }

        public void RaiseSelectedItemChanged(TreeViewItemVM item)
        {
            SelectedItem = item;
            SelectedItemChanged?.Invoke(item);
        }
    }
}