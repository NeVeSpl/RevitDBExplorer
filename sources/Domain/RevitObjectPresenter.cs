using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class RevitObjectPresenter
    {
        public static void Show(IEnumerable<SnoopableObject> snoopableObjects)
        {
            var elementIds = snoopableObjects.Select(x => x.Object).OfType<Element>().Select(x => x.Id).ToList();
            if (elementIds.Any())
            {
                ExternalExecutor.ExecuteInRevitContextAsync(x => { x.ActiveUIDocument?.ShowElements(elementIds); });
            }
        }
        public static bool IsShowInRevitAvailable(SnoopableObject snoopableObject)
        {
            if (snoopableObject.Object is Element)
            {
                return true;
            }
            return false;
        }

        public static void Isolate(IEnumerable<SnoopableObject> snoopableObjects)
        {
            var elementIds = snoopableObjects.Select(x => x.Object).OfType<Element>().Select(x => x.Id).ToList();
            if (elementIds.Any())
            {
                ExternalExecutor.ExecuteInRevitContextAsync(x =>
                {
                    var view = x.ActiveUIDocument?.Document?.ActiveView;
                    if (view is null)
                    {
                        return;
                    }
                    if (view.IsTemporaryHideIsolateActive())
                    {
                        view.DisableTemporaryViewMode(Autodesk.Revit.DB.TemporaryViewMode.TemporaryHideIsolate);
                    }

                    view.IsolateElementsTemporary(elementIds);
                });
            }
        }

        public static void Select(IEnumerable<SnoopableObject> snoopableObjects)
        {
            var elementIds = snoopableObjects.Select(x => x.Object).OfType<Element>().Select(x => x.Id).ToList();
            if (elementIds.Any())
            {
                ExternalExecutor.ExecuteInRevitContextAsync(x => { x.ActiveUIDocument?.Selection.SetElementIds(elementIds); }) ;
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
        public static bool IsSelectInRevitAvailable(SnoopableObject snoopableObject)
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
            if (snoopableObject.Object is Face { Reference : not null})
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