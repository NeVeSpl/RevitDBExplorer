using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
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
                if (Object.Index != -1)
                {
                    return $"[{Object.Index}]";
                }
                if (!string.IsNullOrEmpty(Object.NamePrefix))
                {
                    return Object.NamePrefix;
                }
                return  "";
            }
        }
        public bool IsSelectInRevitAvailable => (Object.Object is Element || Object.Object is Face || Object.Object is Edge || Object.Object is Point || Object.Object is Curve);
        public bool IsShowInRevitAvailable => Object.Object is Element;


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