using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class VisibleInViewFilter : Filter
    {
        private readonly View activeView;


        public VisibleInViewFilter(View activeView)
        {
            this.activeView = activeView;
            Filter = new Autodesk.Revit.DB.VisibleInViewFilter(activeView.Document, activeView.Id);
            FilterSyntax = "new VisibleInViewFilter(document, document.ActiveView.Id)";
        }


        public static IEnumerable<Filter> Create(List<Command> commands, Document document)
        {
            if (commands.Any(x => x.Type == CmdType.ActiveView))
            {
                yield return new VisibleInViewFilter(document.ActiveView);
            }
        }
    }
}