using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree.Items
{
    internal enum GroupBy { None, Root, TypeName, Category, LineStyle, DimensionType, WallType, FloorType, FamilySymbol, Family }


    internal class GroupTreeItem : TreeItem
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
        public GroupBy GroupedBy
        {
            get;
        }
        public string GroupedByToolTip
        {
            get 
            { 
                return Enum.GetName(typeof(GroupBy), GroupedBy);
            }
        }
                

        public GroupTreeItem(ResultOfSnooping resultOfSnooping, Predicate<object> itemFilter, GroupBy groupBy = GroupBy.None)
        {
            GroupedBy = GroupBy.Root;
            Count = resultOfSnooping.Objects.Count;
            Name = resultOfSnooping.Title;

            IEnumerable<GroupTreeItem> groupedItems = null;
            switch (groupBy)
            {
                case GroupBy.TypeName:
                    groupedItems = GroupByTypeName(resultOfSnooping.Objects, itemFilter);
                    break;

                case GroupBy.Category:
                    groupedItems = GroupByCategory(resultOfSnooping.Objects, itemFilter);
                    break;
            }

            SetItems(groupedItems, resultOfSnooping.Objects, itemFilter);
        }
        private void SetItems(IEnumerable<GroupTreeItem> groupedItems, IEnumerable<SnoopableObject> items, Predicate<object> itemFilter)
        {
            if (groupedItems != null)
            {
                Items = new ObservableCollection<TreeItem>(groupedItems.OrderBy(x => x.Name));
            }
            if (Items == null)
            {
                Items = new ObservableCollection<TreeItem>(items.OrderBy(x => x.Index).ThenBy(x => x.Name).Select(x => new SnoopableObjectTreeItem(x)));
            }
            var listCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
            listCollectionView.Filter = itemFilter;
        }


        internal static readonly HashSet<string> NamesToGroupByCategory = new HashSet<string>() { nameof(FamilyInstance), nameof(Element), nameof(FamilySymbol), nameof(IndependentTag), nameof(Family) };


        private GroupTreeItem(string name, IEnumerable<SnoopableObject> items, Predicate<object> itemFilter, GroupBy groupedBy)
        {
            Name = name;
            GroupedBy = groupedBy;
            Count = items.Count();

            IEnumerable<GroupTreeItem> groupedItems = null;

            // 2nd level
            if (GroupedBy == GroupBy.TypeName)
            {
                if (NamesToGroupByCategory.Contains(Name))
                {
                    groupedItems = GroupByCategory(items, itemFilter);
                }

                if (name == nameof(DetailLine))
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as DetailLine).LineStyle, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.LineStyle)).ToList();
                }
                if (name == nameof(Dimension))
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as Dimension).DimensionType, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.DimensionType)).ToList();
                }
                if (name == nameof(Wall))
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as Wall).WallType, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.WallType)).ToList();
                }
                if (name == nameof(Floor))
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as Floor).FloorType, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.FloorType)).ToList();
                }
            }
            // 3rd level
            if (GroupedBy == GroupBy.Category)
            {
                if (items.FirstOrDefault()?.Object is FamilySymbol)
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as FamilySymbol).Family, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.Family)).ToList();
                }
                if (items.FirstOrDefault()?.Object is FamilyInstance)
                {
                    groupedItems = items.GroupBy<SnoopableObject, Element>(x => (x.Object as FamilyInstance).Symbol.Family, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.Family)).ToList();
                }
            }

            SetItems(groupedItems, items, itemFilter);
        }

        private IEnumerable<GroupTreeItem> GroupByTypeName(IEnumerable<SnoopableObject> items, Predicate<object> itemFilter)
        {            
            var groupedItems = items.GroupBy(x => x.TypeName).Select(x => new GroupTreeItem(x.Key, x, itemFilter, GroupBy.TypeName)).OrderBy(x => x.Name);
            return groupedItems;
        }
        private IEnumerable<GroupTreeItem> GroupByCategory(IEnumerable<SnoopableObject> items, Predicate<object> itemFilter)
        {
            IEnumerable<GroupTreeItem> groupedItems = null;           
            if (name == nameof(Family))
            {
                groupedItems = items.GroupBy(x => (x.Object as Family).FamilyCategoryId).Select(x => new GroupTreeItem(Labeler.GetLabelForCategory(x.Key), x, itemFilter, GroupBy.Category));
            }
            else
            {
                groupedItems = items.GroupBy(x => (x.Object as Element).Category?.Id).Select(x => new GroupTreeItem(Labeler.GetLabelForCategory(x.Key), x, itemFilter, GroupBy.Category));
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
                foreach (var group in Items.OfType<GroupTreeItem>())
                {
                    group.Refresh();
                    count += group.Count;
                }
                Count = count + collectionView.OfType<SnoopableObjectTreeItem>().Count(); 
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