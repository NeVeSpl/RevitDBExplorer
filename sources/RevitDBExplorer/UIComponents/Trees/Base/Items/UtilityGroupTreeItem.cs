using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Base.Items
{
    internal class UtilityGroupTreeItem : TreeItem
    {
        public UtilityGroupTreeItem(TreeItemsCommands commands, IEnumerable<SnoopableObject> snoopableObjects) : base(commands)
        {
            Items = new ObservableCollection<TreeItem>(snoopableObjects.Select(x => new SnoopableObjectTreeItem(x, commands)));
        }
    }
}
