using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal enum LogicalOperator { And, Or }


    internal class Group : QueryItem
    {      
        public LogicalOperator Operator { get; init; } = LogicalOperator.Or;
        public IEnumerable<QueryItem> Items { get; init; } = Enumerable.Empty<QueryItem>();


        public Group(IEnumerable<QueryItem> items, LogicalOperator logicalOperator = LogicalOperator.Or)
        {
            Operator = logicalOperator;
            Items = items;

            var filterName = logicalOperator == LogicalOperator.Or ? "LogicalOrFilter" : "LogicalAndFilter";
                          
            FilterSyntax = $"new {filterName}(new [] {{" + String.Join(", ", Items.Select(x => Environment.NewLine + "        " + x.FilterSyntax)) + Environment.NewLine + "    })";
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            if (Operator == LogicalOperator.Or)
            {
                return new LogicalOrFilter(Items.Select(x => x.CreateElementFilter(document)).ToList());
            }
            else
            {
                return new LogicalAndFilter(Items.Select(x => x.CreateElementFilter(document)).ToList());
            }
        }
    }
}