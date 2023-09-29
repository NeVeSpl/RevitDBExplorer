using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Utility
{
    internal class UtilityTreeViewModel : BaseTreeViewModel
    {
        private static UtilityGroupTreeItem rootItem;

        public UtilityTreeViewModel()
        {
            rootItem ??= new UtilityGroupTreeItem(TreeItemsCommands) { IsExpanded = true };
            TreeItems.Add(rootItem);
        }

        internal void AddObject(SnoopableObject inputObject)
        {
            var objectCopy = new SnoopableObject(inputObject.Context.Document, inputObject.Object);
            var vmCopy = new SnoopableObjectTreeItem(objectCopy, TreeItemsCommands);
            rootItem.Items.Add(vmCopy);
        }
    }
}