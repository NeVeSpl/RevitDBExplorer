using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class ElementTypeFilter : Filter
    {
        private readonly bool isElementTypePresent;
        private readonly bool isNotElementTypePresent;


        public ElementTypeFilter(bool isElementTypePresent, bool isNotElementTypePresent)
        {
            this.isElementTypePresent = isElementTypePresent;
            this.isNotElementTypePresent = isNotElementTypePresent;
            if (isElementTypePresent == true && isNotElementTypePresent == false)
            {
                Filter = new ElementIsElementTypeFilter(false);
                FilterSyntax = ".WhereElementIsElementType()";
            }
            if (isElementTypePresent == false && isNotElementTypePresent == true)
            {
                Filter = new ElementIsElementTypeFilter(true);
                FilterSyntax = ".WhereElementIsNotElementType()";
            }
            if (isElementTypePresent == true && isNotElementTypePresent == true)
            {
                Filter = new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false));
                FilterSyntax = "new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false))";
            }
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            var isElementTypePresent = commands.OfType<ElementTypeCmd>().Any();
            var isNotElementTypePresent = commands.OfType<NotElementTypeCmd>().Any();
            if (isElementTypePresent || isNotElementTypePresent)
            {
                yield return new ElementTypeFilter(isElementTypePresent, isNotElementTypePresent);
            }
        }
    }
}