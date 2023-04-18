using System;
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
        private readonly RuleBasedFilterCmdArgument arg;


        public RuleFilter(RuleBasedFilterCmdArgument arg)
        {
            this.arg = arg;
            FilterSyntax = $"new LogicalAndFilter((document.GetElement({arg.Name}) as ParameterFilterElement).GetElementFilter()," + Environment.NewLine;
            FilterSyntax += $"       new ElementMulticategoryFilter((document.GetElement({arg.Name}) as ParameterFilterElement).GetCategories()))";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var rules = commands.OfType<RuleBasedFilterCmd>().SelectMany(x => x.Arguments).OfType<RuleBasedFilterCmdArgument>().ToList();
            if (rules.Count == 1)
            {
                yield return new RuleFilter(rules.First());
            }
            if (rules.Count > 1)
            {
                yield return new Group(rules.Select(x => new RuleFilter(x)).ToList());
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            var ruleElement = document.GetElement(this.arg.Value) as ParameterFilterElement;
            var elementFilter = ruleElement.GetElementFilter();
            var categories = ruleElement.GetCategories();
            var categoryFilter = new ElementMulticategoryFilter(categories);
            var filters = new[] { categoryFilter, elementFilter };
            var andFilter = new LogicalAndFilter(filters.Where(x => x != null).ToArray());
            return andFilter;
        }
    }    
}