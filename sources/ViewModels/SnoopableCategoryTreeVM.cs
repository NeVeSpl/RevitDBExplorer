using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;

namespace RevitDBExplorer.ViewModels
{
    internal class SnoopableCategoryTreeVM : TreeViewItemVM
    {
        private string name;      

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
       

        public SnoopableCategoryTreeVM(string name, IEnumerable<SnoopableObject> items)
        {
            Name = name;
            if (items?.Any() == true)
            {
                if ((items.Count() > 100 && (name == nameof(FamilyInstance) || name == nameof(Element))))
                {
                    var groupedItems = items.GroupBy(x => (x.Object as Element).Category?.Id).Select(x => new SnoopableCategoryTreeVM(GetLabelFor(x.Key), x)).ToList();
                    Items = new ObservableCollection<TreeViewItemVM>(groupedItems.OrderBy(x => x.Name));
                }
                else
                {
                    Items = new ObservableCollection<TreeViewItemVM>(items.OrderBy(x => x.Name).Select(x => new SnoopableObjectTreeVM(x)));
                }
            }
        }

        private static string GetLabelFor(ElementId categoryId)
        {
            if ((categoryId != null) && (Category.IsBuiltInCategoryValid((BuiltInCategory)categoryId.IntegerValue)))
            {
                return LabelUtils.GetLabelFor((BuiltInCategory)categoryId.IntegerValue);
            }
            return "invalid category";
        }
    }
}