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
        private readonly List<ElementIdMatch> ids;


        public ElementIdFilter(List<ElementIdMatch> ids)
        {
            this.ids = ids;
            Filter = new ElementIdSetFilter(ids.Select(x => x.Value).ToList());
            FilterSyntax = "new ElementIdSetFilter(new [] {" + String.Join(", ", ids.Select(x => x.Name)) + "})";
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            var ids = commands.Where(x => x.Type == CmdType.ElementId).SelectMany(x => x.MatchedArguments).OfType<ElementIdMatch>().ToList();
            if (ids.Any())
            {
                yield return new ElementIdFilter(ids);
            }
        }
    }    
}