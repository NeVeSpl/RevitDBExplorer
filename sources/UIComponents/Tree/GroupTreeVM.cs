using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree
{
    internal enum GroupBy { None, TypeName, Category }


    internal class GroupTreeVM : TreeViewItemVM
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


        internal static readonly HashSet<string> NamesToGroupByCategory = new HashSet<string>() { nameof(FamilyInstance), nameof(Element), nameof(FamilySymbol), nameof(IndependentTag), nameof(Family) };

        public GroupTreeVM(string name, IEnumerable<SnoopableObject> items, Predicate<object> itemFilter, GroupBy groupBy = GroupBy.None)
        {
            Name = name;
            Count = items.Count();
           
            if (NamesToGroupByCategory.Contains(Name))
            {
                groupBy = GroupBy.Category;
            }

            IEnumerable<GroupTreeVM> groupedItems = null;
            switch (groupBy)
            {
                case GroupBy.TypeName:
                    groupedItems = GroupByTypeName(items, itemFilter);
                    break;

                case GroupBy.Category:
                    groupedItems = GroupByCategory(items, itemFilter);
                    break;
            }                
               

            if (name == nameof(DetailLine))
            {
                groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as DetailLine).LineStyle, ElementEqualityComparer.Instance).Select(x => new GroupTreeVM(x.Key.Name, x, itemFilter)).ToList();
            }
            if (name == nameof(Dimension))
            {
                groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as Dimension).DimensionType, ElementEqualityComparer.Instance).Select(x => new GroupTreeVM(x.Key.Name, x, itemFilter)).ToList();
            }
            if (name == nameof(Wall))
            {
                groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as Wall).WallType, ElementEqualityComparer.Instance).Select(x => new GroupTreeVM(x.Key.Name, x, itemFilter)).ToList();
            }
            if (name == nameof(Floor))
            {
                groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as Floor).FloorType, ElementEqualityComparer.Instance).Select(x => new GroupTreeVM(x.Key.Name, x, itemFilter)).ToList();
            }


            if (groupedItems != null)
            {                    
                Items = new ObservableCollection<TreeViewItemVM>(groupedItems.OrderBy(x => x.Name));
            }
            if (Items == null)                
            {
                Items = new ObservableCollection<TreeViewItemVM>(items.OrderBy(x => x.Index).ThenBy(x => x.Name).Select(x => new SnoopableObjectTreeVM(x)));
            }


            var listCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
            listCollectionView.Filter = itemFilter;            
        }

        private IEnumerable<GroupTreeVM> GroupByTypeName(IEnumerable<SnoopableObject> items, Predicate<object> itemFilter)
        {            
            var groupedItems = items.GroupBy(x => x.TypeName).Select(x => new GroupTreeVM(x.Key, x, itemFilter)).OrderBy(x => x.Name);
            return groupedItems;
        }
        private IEnumerable<GroupTreeVM> GroupByCategory(IEnumerable<SnoopableObject> items, Predicate<object> itemFilter)
        {
            IEnumerable<GroupTreeVM> groupedItems = null;           
            if (name == nameof(Family))
            {
                groupedItems = items.GroupBy(x => (x.Object as Family).FamilyCategoryId).Select(x => new GroupTreeVM(Labeler.GetLabelForCategory(x.Key), x, itemFilter));
            }
            else
            {
                groupedItems = items.GroupBy(x => (x.Object as Element).Category?.Id).Select(x => new GroupTreeVM(Labeler.GetLabelForCategory(x.Key), x, itemFilter));
            }
            return groupedItems;
        }


       
        public void Refresh()
        {
            if (Items != null)
            {
                var collectionView = CollectionViewSource.GetDefaultView(Items);
                collectionView.Refresh();
                int count = 0;
                foreach (var group in Items.OfType<GroupTreeVM>())
                {
                    group.Refresh();
                    count += group.Count;
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
                return x?.Id?.Value().Equals(y?.Id?.Value()) ?? true;
            }

            public int GetHashCode(Category obj)
            {
                return obj?.Id?.Value().GetHashCode() ?? -1;
            }
        }
    }    
}