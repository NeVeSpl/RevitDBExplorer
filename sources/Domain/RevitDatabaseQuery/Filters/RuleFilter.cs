using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class RuleFilter : Filter
    {
        private readonly RuleMatch ruleMatch;


        public RuleFilter(RuleMatch ruleFilter, Document document)
        {
            this.ruleMatch = ruleFilter;

            var ruleElement = document.GetElement(ruleMatch.Value) as ParameterFilterElement;
            var elementFilter = ruleElement.GetElementFilter();
            Filter = elementFilter;
            FilterSyntax = $"(document.GetElement({ruleFilter.Name}) as ParameterFilterElement).GetElementFilter()";
        }


        public static IEnumerable<QueryItem> Create(List<Command> commands, Document document)
        {
            var rules = commands.Where(x => x.Type == CmdType.RuleBasedFilter).SelectMany(x => x.MatchedArguments).OfType<RuleMatch>().ToList();
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