using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Presentation;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree.Items
{
    internal abstract class TreeItem : BaseViewModel
    {
        private bool isSelected = false;
        private bool isExpanded = false;
        private bool isEnabled  = true;
        private ObservableCollection<TreeItem> items = null;

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
        public SelectInRevitCommand SelectInRevit { get; } = SelectInRevitCommand.Instance;
        public ShowInRevitCommand ShowInRevit { get; } = ShowInRevitCommand.Instance;
        public SnoopInNewWindowCommand SnoopInNewWindow { get; } = SnoopInNewWindowCommand.Instance;
        public IsolateInRevitCommand IsolateInRevit { get; } = IsolateInRevitCommand.Instance;
        public DrawInRevitCommand DrawInRevit { get; } = DrawInRevitCommand.Instance;
        public ObservableCollection<TreeItem> Items
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
            TreeItem candidate = null;

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
        public IEnumerable<SnoopableObject> GetAllSnoopableObjects()
        {
            if (this is SnoopableObjectTreeItem snoopableObjectTreeVM)
            {
                yield return snoopableObjectTreeVM.Object;
            }
            if (Items != null)
            {
                var collectionView = CollectionViewSource.GetDefaultView(Items);
                foreach (var item in collectionView.OfType<SnoopableObjectTreeItem>())
                {
                    if (item.Object != null)
                    {
                        yield return item.Object;
                    }
                }
                foreach (var group in Items.OfType<GroupTreeItem>())
                {
                    foreach (var item in group.GetAllSnoopableObjects())
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}