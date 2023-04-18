using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopDatabase : ISelector
    {
        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var elementTypes = new FilteredElementCollector(document).WhereElementIsElementType();
            var elementInstances = new FilteredElementCollector(document).WhereElementIsNotElementType();
            var elementsCollector = elementTypes.UnionWith(elementInstances);
            var elements = elementsCollector.ToElements();

            return elements.Select(x => new SnoopableObject(document, x));
        }
    }
}