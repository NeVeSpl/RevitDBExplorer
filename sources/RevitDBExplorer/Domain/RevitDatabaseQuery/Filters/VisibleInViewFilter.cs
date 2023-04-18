using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class VisibleInViewFilter : Filter
    {
        public VisibleInViewFilter()
        {                  
            FilterSyntax = "new VisibleInViewFilter(document, document.ActiveView.Id)";
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            if (commands.OfType<VisibleInViewCmd>().Any())
            {
                yield return new VisibleInViewFilter();
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new Autodesk.Revit.DB.VisibleInViewFilter(document, document.ActiveView.Id);
        }
    }
}