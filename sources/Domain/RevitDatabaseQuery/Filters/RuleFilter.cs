using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class RuleFilter : Filter
    {
        private readonly RuleBasedFilterCmdArgument ruleMatch;


        public RuleFilter(RuleBasedFilterCmdArgument ruleFilter, Document document)
        {
            this.ruleMatch = ruleFilter;

            var ruleElement = document.GetElement(ruleMatch.Value) as ParameterFilterElement;
            var elementFilter = ruleElement.GetElementFilter();
            Filter = elementFilter;
            FilterSyntax = $"(document.GetElement({ruleFilter.Name}) as ParameterFilterElement).GetElementFilter()";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands, Document document)
        {
            var rules = commands.OfType<RuleBasedFilterCmd>().SelectMany(x => x.Arguments).OfType<RuleBasedFilterCmdArgument>().ToList();
            if (rules.Count == 1)
            {
                yield return new RuleFilter(rules.First(), document);
            }
            if (rules.Count > 1)
            {
                yield return new Group(rules.Select(x => new RuleFilter(x, document)).ToList());
            }
        }
    }    
}