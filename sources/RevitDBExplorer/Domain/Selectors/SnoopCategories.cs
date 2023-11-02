using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopCategories : ISelector
    {
        public InfoAboutSource Info { get; } = new("ParameterFilterUtilities.GetAllFilterableCategories()");


        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) return null;

            var ids = ParameterFilterUtilities.GetAllFilterableCategories();
            var categorries = ids.Select(x => Category.GetCategory(document, x)).Where(x => x is not null).ToList();

            return categorries.Select(x => new SnoopableObject(document, x));
        }
    }
}