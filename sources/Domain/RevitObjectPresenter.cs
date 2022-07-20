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
                    ExternalExecutor.ExecuteInRevitContextAsync(x => { x.ActiveUIDocument?.Selection.SetReferences(new[] { reference }); });
                }
            }
        }
    }
}