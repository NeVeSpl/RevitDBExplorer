using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Base.Items
{
    internal class UtilityGroupTreeItem : TreeItem
    {
        public UtilityGroupTreeItem(TreeItemsCommands commands) : base(commands)
        {
            Items = new System.Collections.ObjectModel.ObservableCollection<TreeItem>();
        }
    }
}
