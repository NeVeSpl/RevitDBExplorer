using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class CategoryFilter : Filter
    {
        private readonly List<CategoryCmdArgument> categories;


        public CategoryFilter(List<CategoryCmdArgument> categories)
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
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            var categories = commands.OfType<CategoryCmd>().SelectMany(x => x.Arguments).OfType<CategoryCmdArgument>().ToList();
            if (categories.Any())
            {
                yield return new CategoryFilter(categories);
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementMulticategoryFilter(categories.Select(x => x.Value).ToList());
        }
    }
}