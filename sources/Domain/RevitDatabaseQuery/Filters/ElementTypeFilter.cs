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
        private enum Version { Element, Type, Both }

        private readonly Version version;

        public ElementTypeFilter(bool isElementTypePresent, bool isNotElementTypePresent)
        {           
            if (isElementTypePresent == true && isNotElementTypePresent == false)
            {
                version = Version.Type;
                FilterSyntax = ".WhereElementIsElementType()";
            }
            if (isElementTypePresent == false && isNotElementTypePresent == true)
            {
                version = Version.Element;                
                FilterSyntax = ".WhereElementIsNotElementType()";
            }
            if (isElementTypePresent == true && isNotElementTypePresent == true)
            {
                version = Version.Both;               
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

        public override ElementFilter CreateElementFilter(Document document)
        {
            return version switch
            {
                Version.Element => new ElementIsElementTypeFilter(true),
                Version.Type => new ElementIsElementTypeFilter(false),
                Version.Both => new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)),
            };
        }
    }
}