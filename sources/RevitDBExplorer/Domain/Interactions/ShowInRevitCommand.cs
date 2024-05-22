using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Interactions
{
    internal class ShowInRevitCommand : BaseCommand
    {
        public static readonly ShowInRevitCommand Instance = new ShowInRevitCommand();


        public override bool CanExecute(object parameter)
        {
            if (parameter is TreeItem treeViewItem)
            {
                var elements = treeViewItem.GetAllSnoopableObjects().Where(x => x.Object is Element);

                if (elements.Any())
                {
                    return true;
                }
            }
            return false;
        }

        public override void Execute(object parameter)
        {
            if (parameter is TreeItem treeViewItem)
            {
                var elementIds = treeViewItem.GetAllSnoopableObjects().Select(x => x.Object).OfType<Element>().Select(x => x.Id).ToList();
                if (elementIds.Any())
                {
                    ExternalExecutor.ExecuteInRevitContextAsync(x => { x.ActiveUIDocument?.ShowElements(elementIds); });
                }
            }
          
        }
    }
}