using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class ElementIdFilter : Filter
    {
        private readonly List<ElementIdCmdArgument> ids;


        public ElementIdFilter(List<ElementIdCmdArgument> ids)
        {
            this.ids = ids;          
            FilterSyntax = "new ElementIdSetFilter(new [] {" + String.Join(", ", ids.Select(x => x.Name)) + "})";
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            var ids = commands.OfType<ElementIdCmd>().SelectMany(x => x.Arguments).OfType<ElementIdCmdArgument>().ToList();
            if (ids.Any())
            {
                yield return new ElementIdFilter(ids);
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementIdSetFilter(ids.Select(x => x.Value).ToList());
        }
    }    
}