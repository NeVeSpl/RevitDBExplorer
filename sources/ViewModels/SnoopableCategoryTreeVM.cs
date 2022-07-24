using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.ViewModels
{
    internal class SnoopableCategoryTreeVM : TreeViewItemVM
    {
        private string name;
        private int count;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                OnPropertyChanged();
            }
        }


        public SnoopableCategoryTreeVM(string name, IEnumerable<SnoopableObject> items, Predicate<object> itemFilter)
        {
            Name = name;
            Count = items.Count();
            if (items?.Any() == true)
            {
                List<SnoopableCategoryTreeVM> groupedItems = null;
                if (name == nameof(FamilyInstance) || name == nameof(Element) || name == nameof(FamilySymbol) || name == nameof(IndependentTag))
                {
                    groupedItems = items.GroupBy(x => (x.Object as Element).Category?.Id).Select(x => new SnoopableCategoryTreeVM(Labeler.GetLabelForCategory(x.Key), x, itemFilter)).ToList();
                }
                if (name == nameof(Family))
                {
                    groupedItems = items.GroupBy(x => (x.Object as Family).FamilyCategoryId).Select(x => new SnoopableCategoryTreeVM(Labeler.GetLabelForCategory(x.Key), x, itemFilter)).ToList();
                }
                if (name == nameof(DetailLine))
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as DetailLine).LineStyle, ElementEqualityComparer.Instance).Select(x => new SnoopableCategoryTreeVM(x.Key.Name, x, itemFilter)).ToList();
                }
                if (name == nameof(Dimension))
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as Dimension).DimensionType, ElementEqualityComparer.Instance).Select(x => new SnoopableCategoryTreeVM(x.Key.Name, x, itemFilter)).ToList();
                }
                if (name == nameof(Wall))
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as Wall).WallType, ElementEqualityComparer.Instance).Select(x => new SnoopableCategoryTreeVM(x.Key.Name, x, itemFilter)).ToList();
                }
                if (name == nameof(Floor))
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as Floor).FloorType, ElementEqualityComparer.Instance).Select(x => new SnoopableCategoryTreeVM(x.Key.Name, x, itemFilter)).ToList();
                }
                if (groupedItems != null)
                {                    
                    Items = new ObservableCollection<TreeViewItemVM>(groupedItems.OrderBy(x => x.Name));
                }
                if (Items == null)                
                {
                    Items = new ObservableCollection<TreeViewItemVM>(items.OrderBy(x => x.Index).ThenBy(x => x.Name).Select(x => new SnoopableObjectTreeVM(x)));
                }

                var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
                lcv.Filter = itemFilter;
            }
        }


        public void Refresh()
        {
            if (Items != null)
            {
                var collectionView = CollectionViewSource.GetDefaultView(Items);
                collectionView.Refresh();
                int count = 0;
                foreach (SnoopableCategoryTreeVM item in Items.OfType<SnoopableCategoryTreeVM>())
                {
                    item.Refresh();
                    count += item.Count;
                }
                Count = count + collectionView.OfType<SnoopableObjectTreeVM>().Count(); 
            }
        }   
        

        class ElementEqualityComparer : IEqualityComparer<Element>
        {
            public static readonly ElementEqualityComparer Instance = new();

            public bool Equals(Element x, Element y)
            {
                return x?.Id?.Equals(y?.Id) ?? true;
            }

            public int GetHashCode(Element obj)
            {
                return obj?.Id?.GetHashCode() ?? -1;
            }
        }
        class CategoryEqualityComparer : IEqualityComparer<Category>
        {
            public static readonly CategoryEqualityComparer Instance = new();

            public bool Equals(Category x, Category y)
            {
                return x?.Id?.IntegerValue.Equals(y?.Id?.IntegerValue) ?? true;
            }

            public int GetHashCode(Category obj)
            {
                return obj?.Id?.IntegerValue.GetHashCode() ?? -1;
            }
        }
    }    
}