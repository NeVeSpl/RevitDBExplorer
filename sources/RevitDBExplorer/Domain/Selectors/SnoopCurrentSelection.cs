using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopCurrentSelection : ISelector
    {
        public InfoAboutSource Info { get; private set; } = new("<s>");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var selectedIds = app.ActiveUIDocument.Selection.GetElementIds();

            FilteredElementCollector collector = null;

            if (selectedIds.Any())
            {
                collector = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(selectedIds));
                Info.ShortTitle = "selected elements";
            }
            else
            {
                collector = new FilteredElementCollector(document, document.ActiveView.Id);
                Info.ShortTitle = "visible elements in a view";               
            }

            if (collector.GetElementCount() == 0 && selectedIds.Count > 0)
            {
                var someOutliers = new List<SnoopableObject>(selectedIds.Count);
                foreach (var id in selectedIds)
                {
                    var element = document.GetElement(id);
                    if (element != null) someOutliers.Add(new SnoopableObject(document, element));
                }
                return someOutliers;
            }
            
            return collector.ToElements().Select(x => new SnoopableObject(document, x));
        }
    }
}