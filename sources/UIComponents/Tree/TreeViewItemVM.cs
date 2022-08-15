using System.Collections.ObjectModel;
using System.Linq;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree
{
    internal class TreeViewItemVM : BaseViewModel
    {
        private bool isSelected = false;
        private bool isExpanded = false;
        private bool isEnabled  = true;
        private ObservableCollection<TreeViewItemVM> items = null;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;
                OnPropertyChanged();
            }
        }
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        
        public ObservableCollection<TreeViewItemVM> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }



        public void Expand(bool isSingleChild = false, int maxHeight = 33)
        {
            if (isSingleChild || maxHeight > 0)
            {
                IsExpanded = true;
            }
            if ((IsExpanded) && (Items != null))
            {
                var allChildren = Items.Sum(x => x.Items?.Count) ?? 0;
                foreach (var item in Items)
                {
                    item.Expand(Items.Count == 1, maxHeight - Items.Count - allChildren);
                }
            }
        }
        public void SelectFirstDeepestVisibleItem()
        {
            TreeViewItemVM candidate = null;

            if (isExpanded)
            {
                candidate = Items?.FirstOrDefault();
            }
           
            if (candidate != null)
            {
                candidate.SelectFirstDeepestVisibleItem();
            }
            else
            {
                IsSelected = true;
            }
        }
    }
}