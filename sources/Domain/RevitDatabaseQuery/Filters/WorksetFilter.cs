using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class WorksetFilter : Filter
    {
        private readonly WorksetCmdArgument arg;


        public WorksetFilter(WorksetCmdArgument arg)
        {
            this.arg = arg;
            FilterSyntax = $"new ElementWorksetFilter ({arg.Name})";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var worksets = commands.OfType<WorksetCmd>().SelectMany(x => x.Arguments).OfType<WorksetCmdArgument>().ToList();
            if (worksets.Count == 1)
            {
                yield return new WorksetFilter(worksets.First());
            }
            if (worksets.Count > 1)
            {
                yield return new Group(worksets.Select(x => new WorksetFilter(x)).ToList());
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementWorksetFilter(arg.Value);
        }
    }
}
