using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class CategoryFilter : Filter
    {
        private readonly List<CategoryMatch> categories;


        public CategoryFilter(List<CategoryMatch> categories)
        {
            this.categories = categories;
            if (categories.Count == 1)
            {               
                FilterSyntax = $".OfCategory({categories.First().Name})";
            }
            else
            {               
                FilterSyntax = "new ElementMulticategoryFilter(new [] {" + String.Join(", ", categories.Select(x => x.Name)) + "})";
            }
            Filter = new ElementMulticategoryFilter(categories.Select(x => x.Value).ToList());
        }


        public static IEnumerable<Filter> Create(List<Command> commands)
        {
            var categories = commands.Where(x => x.Type == CmdType.Category).SelectMany(x => x.MatchedArguments).OfType<CategoryMatch>().ToList();
            if (categories.Any())
            {
                yield return new CategoryFilter(categories);
            }
        }
    }
}