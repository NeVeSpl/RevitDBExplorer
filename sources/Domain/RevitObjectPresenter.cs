using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class RevitObjectPresenter
    {
      

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

       
    }
}