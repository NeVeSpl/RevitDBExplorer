using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopSchemas : ISelector
    {
        public InfoAboutSource Info { get; } = new("Schema.ListSchemas()");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            return Schema.ListSchemas().Select(x => new SnoopableObject(app?.ActiveUIDocument?.Document, x));
        }
    }
}