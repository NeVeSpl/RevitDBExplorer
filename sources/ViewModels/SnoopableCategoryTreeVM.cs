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
                if (name == nameof(FamilyInstance) || name == nameof(Element) || name == nameof(FamilySymbol))
                {
                    var groupedItems = items.GroupBy(x => (x.Object as Element).Category?.Id).Select(x => new SnoopableCategoryTreeVM(Labeler.GetLabelForCategory(x.Key), x, itemFilter)).ToList();
                    Items = new ObservableCollection<TreeViewItemVM>(groupedItems.OrderBy(x => x.Name));
                }
                if (name == nameof(Family))
                {
                    var groupedItems = items.GroupBy(x => (x.Object as Family).FamilyCategoryId).Select(x => new SnoopableCategoryTreeVM(Labeler.GetLabelForCategory(x.Key), x, itemFilter)).ToList();
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
    }
}