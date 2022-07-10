using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                Items = new ObservableCollection<TreeViewItemVM>(items.OrderBy(x => x.Name).Select(x => new SnoopableObjectTreeVM(x)));
            }
        }
    }
}