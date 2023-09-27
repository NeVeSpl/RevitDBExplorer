using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Base.Items
{
    internal enum GroupBy { None, Source, TypeName, Category, LineStyle, DimensionType, WallType, FloorType, FamilySymbol, Family }


    internal class GroupTreeItem : TreeItem
    {
        private string name;
        private int count;
        private GroupTreeItem parent;


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
            get; set;
        }
        public virtual string GroupedByToolTip
        {
            get
            {
                return Enum.GetName(typeof(GroupBy), GroupedBy);
            }
        }
        public GroupTreeItem Parent => parent;


        public GroupTreeItem(SourceOfObjects sourceOfObjects, Predicate<object> itemFilter, GroupBy groupBy, TreeItemsCommands commands) : base(commands)
        {
            GroupedBy = GroupBy.Source;
            Count = sourceOfObjects.Objects.Count;
            Name = sourceOfObjects.Title;

            IEnumerable<GroupTreeItem> groupedItems = null;
            switch (groupBy)
            {
                case GroupBy.TypeName:
                    groupedItems = GroupByTypeName(sourceOfObjects.Objects, itemFilter, this);
                    break;

                case GroupBy.Category:
                    groupedItems = GroupByCategory(sourceOfObjects.Objects, itemFilter, this);
                    break;
            }

            SetItems(groupedItems, sourceOfObjects.Objects, itemFilter);
        }
        private void SetItems(IEnumerable<GroupTreeItem> groupedItems, IEnumerable<SnoopableObject> items, Predicate<object> itemFilter)
        {
            if (groupedItems != null)
            {
                Items = new ObservableCollection<TreeItem>(groupedItems.OrderBy(x => x.Name));
            }
            if (Items == null)
            {
                Items = new ObservableCollection<TreeItem>(items.OrderBy(x => x.Index).ThenBy(x => x.Name).Select(x => new SnoopableObjectTreeItem(x, Commands)));
            }
            var listCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
            listCollectionView.Filter = itemFilter;
        }


        internal static readonly HashSet<string> NamesToGroupByCategory = new HashSet<string>() { nameof(FamilyInstance), nameof(Element), nameof(FamilySymbol), nameof(IndependentTag), nameof(Family) };


        protected GroupTreeItem(string name, IEnumerable<SnoopableObject> items, Predicate<object> itemFilter, GroupBy groupedBy, GroupTreeItem parent) : base(parent.Commands)
        {
            this.parent = parent;
            Name = name;
            GroupedBy = groupedBy;
            Count = items.Count();

            IEnumerable<GroupTreeItem> groupedItems = null;

            // 2nd level
            if (parent?.GroupedBy == GroupBy.Source && GroupedBy == GroupBy.TypeName)
            {
                if (NamesToGroupByCategory.Contains(Name))
                {
                    groupedItems = GroupByCategory(items, itemFilter, this);
                }
            }
            if (parent?.GroupedBy == GroupBy.Source && GroupedBy == GroupBy.Category)
            {
                groupedItems = GroupByTypeName(items, itemFilter, this);
            }

            if (GroupedBy == GroupBy.TypeName)
            {
                if (name == nameof(DetailLine))
                {
                    groupedItems = items.GroupBy(x => (x.Object as DetailLine).LineStyle, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.LineStyle, this)).ToList();
                }
                if (name == nameof(Dimension))
                {
                    groupedItems = items.GroupBy(x => (x.Object as Dimension).DimensionType, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.DimensionType, this)).ToList();
                }
                if (name == nameof(Wall))
                {
                    groupedItems = items.GroupBy(x => (x.Object as Wall).WallType, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.WallType, this)).ToList();
                }
                if (name == nameof(Floor))
                {
                    groupedItems = items.GroupBy(x => (x.Object as Floor).FloorType, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key.Name, x, itemFilter, GroupBy.FloorType, this)).ToList();
                }
                if (name == nameof(Rebar))
                {
                    // todo : group by rebar number BuiltInParameter.REBAR_NUMBER(-1154616)
                }
            }
            // 3rd level
            if (parent?.GroupedBy == GroupBy.TypeName && GroupedBy == GroupBy.Category || parent?.GroupedBy == GroupBy.Category && GroupedBy == GroupBy.TypeName)
            {
                if (items.FirstOrDefault()?.Object is FamilySymbol)
                {
                    groupedItems = items.GroupBy(x => (x.Object as FamilySymbol)?.Family, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key?.Name, x, itemFilter, GroupBy.Family, this)).ToList();
                }
                if (items.FirstOrDefault()?.Object is FamilyInstance)
                {
                    groupedItems = items.GroupBy(x => (x.Object as FamilyInstance)?.Symbol?.Family, ElementEqualityComparer.Instance).Select(x => new GroupTreeItem(x.Key?.Name, x, itemFilter, GroupBy.Family, this)).ToList();
                }
            }
            //


            SetItems(groupedItems, items, itemFilter);
        }

        private IEnumerable<GroupTreeItem> GroupByTypeName(IEnumerable<SnoopableObject> items, Predicate<object> itemFilter, GroupTreeItem parent)
        {
            var groupedItems = items.GroupBy(x => x.TypeName).Select(x => new TypeGroupTreeItem(x.Key, x, itemFilter, parent)).OrderBy(x => x.Name);
            return groupedItems;
        }
        private IEnumerable<GroupTreeItem> GroupByCategory(IEnumerable<SnoopableObject> items, Predicate<object> itemFilter, GroupTreeItem parent)
        {
            IEnumerable<GroupTreeItem> groupedItems = null;
            if (name == nameof(Family))
            {
                groupedItems = items.GroupBy(x => (x.Object as Family).FamilyCategoryId).Select(x => new CategoryGroupTreeItem(Labeler.GetLabelForCategory(x.Key), x, itemFilter, parent, x.Key));
            }
            else
            {
                groupedItems = items.GroupBy(x => (x.Object as Element)?.Category?.Id).Select(x => new CategoryGroupTreeItem(Labeler.GetLabelForCategory(x.Key), x, itemFilter, parent, x.Key));
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

    internal class CategoryGroupTreeItem : GroupTreeItem
    {
        private readonly ElementId categoryId;
        private readonly string tooltip = "Category";

        public override string GroupedByToolTip
        {
            get
            {
                return tooltip;
            }
        }



        public CategoryGroupTreeItem(string name, IEnumerable<SnoopableObject> items, Predicate<object> itemFilter, GroupTreeItem parent, ElementId categoryId) : base(name, items, itemFilter, GroupBy.Category, parent)
        {
            this.categoryId = categoryId;
            if (categoryId != null)
            {
                tooltip = $"Category: {Enum.GetName(typeof(BuiltInCategory), categoryId?.Value())}({categoryId?.Value()})";
            }
        }
    }

    internal class TypeGroupTreeItem : GroupTreeItem
    {
        private readonly string typeName;

        public TypeGroupTreeItem(string name, IEnumerable<SnoopableObject> items, Predicate<object> itemFilter, GroupTreeItem parent) : base(name, items, itemFilter, GroupBy.TypeName, parent)
        {
            typeName = name;
        }
    }
}