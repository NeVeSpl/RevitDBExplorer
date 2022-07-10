using System.Collections.ObjectModel;
using System.Linq;
using RevitDBExplorer.Domain.DataModel;

namespace RevitDBExplorer.ViewModels
{
    internal class SnoopableObjectTreeVM : TreeViewItemVM
    {
        public SnoopableObject Object { get; }      


        public SnoopableObjectTreeVM(SnoopableObject @object)
        {
            Object = @object;
            if (@object.Items?.Any() == true)
            {
                Items = new ObservableCollection<TreeViewItemVM>(@object.Items.Select(x => new SnoopableObjectTreeVM(x)));
            }
        }
    }
}