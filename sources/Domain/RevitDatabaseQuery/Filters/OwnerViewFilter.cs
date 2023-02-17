using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class OwnerViewFilter : Filter
    {       
        public OwnerViewFilter(View view)
        {          
            Filter = new Autodesk.Revit.DB.ElementOwnerViewFilter(view.Id);
            FilterSyntax = "new ElementOwnerViewFilter(document.ActiveView.Id)";
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands, Document document)
        {
            if (commands.OfType<OwnerViewFilterCmd>().Any())
            {
                yield return new OwnerViewFilter(document.ActiveView);
            }
        }
    }
}