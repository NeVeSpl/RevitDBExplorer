using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class ClassFilter : Filter
    {
        private readonly List<ClassMatch> types;
        

        public ClassFilter(List<ClassMatch> types)
        {
            this.types = types;
            if (types.Count == 1)
            {               
                FilterSyntax = $".OfClass({types.First().Name})";
            }
            else
            {                
                FilterSyntax = "new ElementMulticlassFilter(new [] {" + String.Join(", ", types.Select(x => x.Name)) + "})";
            }
            Filter = new ElementMulticlassFilter(types.Select(x => x.Value).ToList());
        }


        public static IEnumerable<Filter> Create(List<Command> commands)
        {
            var types = commands.Where(x => x.Type == CmdType.Class).SelectMany(x => x.MatchedArguments).OfType<ClassMatch>().ToList();
            if (types.Any())
            {
                yield return new ClassFilter(types);
            }
        }
    }
}