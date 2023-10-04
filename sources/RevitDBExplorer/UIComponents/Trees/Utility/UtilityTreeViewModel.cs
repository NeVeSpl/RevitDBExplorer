using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Utility
{
    internal class UtilityTreeViewModel : BaseTreeViewModel
    {
        private static UtilityGroupTreeItem rootItem;

        public RelayCommand RemoveCommand { get; }


        public UtilityTreeViewModel()
        {
            AllowToFrezeeItem = true;
            rootItem ??= new UtilityGroupTreeItem(TreeItemsCommands) { IsExpanded = true };
            TreeItems.Add(rootItem);
            RemoveCommand = new RelayCommand(RemoveItems);
        }


        private void RemoveItems(object item)
        {
            if (item is SnoopableObjectTreeItem snoopableObjectTreeItem)
            {
                rootItem.Items.Remove(snoopableObjectTreeItem);                
            }
            else
            {
                if (SelectedItem != null)
                {
                    SelectedItem.IsSelected = false;
                }
                rootItem.Items.Clear();
            }    
        }
        public void AddObject(SnoopableObject inputObject)
        {
            var objectCopy = new SnoopableObject(inputObject.Context.Document, inputObject.Object);
            var vmCopy = new SnoopableObjectTreeItem(objectCopy, TreeItemsCommands);
            rootItem.Items.Add(vmCopy);
        }
        public void MoveItem(SnoopableObjectTreeItem item, SnoopableObjectTreeItem target)
        {
            var oldIndex = rootItem.Items.IndexOf(item);
            var newIndex = rootItem.Items.IndexOf(target);

            rootItem.Items.Move(oldIndex, newIndex);
        }
    }
}