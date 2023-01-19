using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopDependentElements : ISelector
    {
        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var selectedIds = app.ActiveUIDocument.Selection.GetElementIds();
            if (!selectedIds.Any())
            {
                return null;
            }

            var selectedElements = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(selectedIds)).ToElements();
            var dependentElementIds = selectedElements.SelectMany(x => x.GetDependentElements(null)).ToList();
            var elements = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(dependentElementIds)).ToElements();
            return elements.Select(x => new SnoopableObject(document, x));
        }
    }
}