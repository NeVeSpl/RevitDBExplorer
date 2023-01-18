using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Tree;
using RevitDBExplorer.UIComponents.Tree.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Presentation
{
    internal class SelectInRevitCommand : BaseCommand
    {
        public static readonly SelectInRevitCommand Instance = new SelectInRevitCommand();


        public override bool CanExecute(object parameter)
        {
            if (parameter is TreeItem treeViewItem)
            {
                var isAvailable = treeViewItem.GetAllSnoopableObjects().All(x => IsSelectInRevitAvailable(x));

                if (isAvailable)
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
                var snoopableObjects = treeViewItem.GetAllSnoopableObjects().ToList();
                var elementIds = snoopableObjects.Select(x => x.Object).OfType<Element>().Select(x => x.Id).ToList();
                if (elementIds.Any())
                {
                    ExternalExecutor.ExecuteInRevitContextAsync(x => { x.ActiveUIDocument?.Selection.SetElementIds(elementIds); });
                }
                var geometryObjects = snoopableObjects.Select(x => x.Object).OfType<GeometryObject>().ToList();
                if (geometryObjects.Any())
                {
                    var references = geometryObjects.Select(x => x.GetReference()).ToList();
                    if (references.Any())
                    {
#if R2023b
                        ExternalExecutor.ExecuteInRevitContextAsync(x => { x.ActiveUIDocument?.Selection.SetReferences(references); });
#endif
                    }
                }
            }
          
        }


        private bool IsSelectInRevitAvailable(SnoopableObject snoopableObject)
        {
            if (snoopableObject.Object is Element)
            {
                return true;
            }
            if (snoopableObject.Object is Curve { Reference: not null })
            {
                return true;
            }
            if (snoopableObject.Object is Edge { Reference: not null })
            {
                return true;
            }
            if (snoopableObject.Object is Face { Reference: not null })
            {
                return true;
            }
            if (snoopableObject.Object is Point { Reference: not null })
            {
                return true;
            }

            return false;
        }
    }
}