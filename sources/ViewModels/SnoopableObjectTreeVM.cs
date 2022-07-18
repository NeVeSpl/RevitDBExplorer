using System.Collections.ObjectModel;
using System.Linq;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.ViewModels
{
    internal class SnoopableObjectTreeVM : TreeViewItemVM
    {
        public SnoopableObject Object { get; }     
        public string Prefix
        {
            get
            {
                return Object.Index != -1 ? $"[{Object.Index}]" : "";
            }
        }


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