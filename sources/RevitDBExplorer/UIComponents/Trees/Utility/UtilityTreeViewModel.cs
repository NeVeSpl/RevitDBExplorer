using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Utility
{
    internal class UtilityTreeViewModel : BaseTreeViewModel
    {


        public UtilityTreeViewModel()
        {
            TreeItems.Add(new UtilityGroupTreeItem(TreeItemsCommands) {IsExpanded = true });
        }

        internal void AddObject(SnoopableObject input)
        {
            var copy = new SnoopableObjectTreeItem(input, TreeItemsCommands);
            TreeItems.First().Items.Add(copy);
        }
    }
}
