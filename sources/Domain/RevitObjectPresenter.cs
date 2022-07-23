using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class RevitObjectPresenter
    {

        public static void Show(SnoopableObject snoopableObject)
        {
            if (snoopableObject.Object is Element element)
            {
                ExternalExecutor.ExecuteInRevitContextAsync(x => { x.ActiveUIDocument?.ShowElements(new[] { element.Id }); });
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

        public static void Isolate(SnoopableObject snoopableObject)
        {
            if (snoopableObject.Object is Element element)
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

                    view.IsolateElementsTemporary(new[] { element.Id });
                });
            }
        }

        public static void Select(SnoopableObject snoopableObject)
        {
            if (snoopableObject.Object is Element element)
            {
                ExternalExecutor.ExecuteInRevitContextAsync(x => { x.ActiveUIDocument?.Selection.SetElementIds(new[] { element.Id }); }) ;
            }
            if (snoopableObject.Object is GeometryObject geometryObject)
            {
                var reference = geometryObject.GetReference();
                if (reference != null)
                {
#if R2023
                    ExternalExecutor.ExecuteInRevitContextAsync(x => { x.ActiveUIDocument?.Selection.SetReferences(new[] { reference }); });
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