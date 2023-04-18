using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class ClassFilter : Filter
    {
        private readonly List<ClassCmdArgument> types;
        

        public ClassFilter(List<ClassCmdArgument> types)
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
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            var types = commands.OfType<ClassCmd>().SelectMany(x => x.Arguments).OfType<ClassCmdArgument>().ToList();
            if (types.Any())
            {
                yield return new ClassFilter(types);
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementMulticlassFilter(types.Select(x => x.Value).ToList());
        }
    }    
}