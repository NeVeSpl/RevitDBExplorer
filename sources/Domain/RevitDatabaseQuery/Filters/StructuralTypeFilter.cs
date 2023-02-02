using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class StructuralTypeFilter : Filter
    {
        private readonly StructuralTypeMatch structuralType;


        public StructuralTypeFilter(StructuralTypeMatch structuralType)
        {
            this.structuralType = structuralType;           
            Filter = new ElementStructuralTypeFilter(structuralType.Value);
            FilterSyntax = $"new ElementStructuralTypeFilter({structuralType.Name})";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var structuralTypes = commands.Where(x => x.Type == CmdType.StructuralType).SelectMany(x => x.MatchedArguments).OfType<StructuralTypeMatch>().ToList();
            if (structuralTypes.Count == 1)
            {
                yield return new StructuralTypeFilter(structuralTypes.First());
            }
            if (structuralTypes.Count > 1)
            {
                yield return new Group(structuralTypes.Select(x => new StructuralTypeFilter(x)).ToList());
            }
        }
    }   
}