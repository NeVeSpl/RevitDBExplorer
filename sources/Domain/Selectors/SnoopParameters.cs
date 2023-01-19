using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.Selectors.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Selectors
{
    internal class SnoopParameters : ISelector
    {
        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            var document = app?.ActiveUIDocument?.Document;

            if (document == null) yield break;

            var categoryIds = ParameterFilterUtilities.GetAllFilterableCategories();
            var categorries = categoryIds.Select(x => Category.GetCategory(document, x)).Where(x => x is not null).ToList();
            foreach (var category in categorries)
            {
                var paramIds = ParameterFilterUtilities.GetFilterableParametersInCommon(document, new[] { category.Id });
                IEnumerable<SnoopableObject> snoopableParameters = Enumerable.Empty<SnoopableObject>();
                if (paramIds.Any())
                {
                    var parameters = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(paramIds)).ToList();
                    snoopableParameters = parameters.Select(x => new SnoopableObject(document, x));
                }

                yield return new SnoopableObject(document, category, snoopableParameters);
            }
        }
    }
}