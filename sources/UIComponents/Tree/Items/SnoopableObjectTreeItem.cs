using System.Collections.ObjectModel;
using System.Linq;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree.Items
{
    internal class SnoopableObjectTreeItem : TreeItem
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


        public SnoopableObjectTreeItem(SnoopableObject @object)
        {
            Object = @object;
            if (@object.Items?.Any() == true)
            {
                Items = new ObservableCollection<TreeItem>(@object.Items.Select(x => new SnoopableObjectTreeItem(x)));
            }
        }
    }
}