using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

namespace RevitDBExplorer.Domain.Interactions
{
    internal class FreezeCommand : BaseCommand
    {
        public static readonly FreezeCommand Instance = new FreezeCommand();

        public override bool CanExecute(object parameter)
        {
            if (parameter is SnoopableObjectTreeItem treeViewItem)
            {
               if (treeViewItem.Object is SnoopableObject snoopableObject)
               {
                   return !snoopableObject.IsFrozen;
               }
            }
            return false;
        }
        public override void Execute(object parameter)
        {
            if (parameter is SnoopableObjectTreeItem treeViewItem)
            {
                if (treeViewItem.Object is SnoopableObject snoopableObject)
                {
                    snoopableObject.Freeze();
                }
            }
        }
    }
}