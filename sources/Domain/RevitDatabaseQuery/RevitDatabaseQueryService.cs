using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;
using VisibleInViewFilter = RevitDBExplorer.Domain.RevitDatabaseQuery.Filters.VisibleInViewFilter;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal static class RevitDatabaseQueryService
    {
        public static void Init()
        {
            FuzzySearchEngine.Init();
        }


        public static Result Parse(Document document, string query)
        {
            FuzzySearchEngine.LoadDocumentSpecificData(document);
            var commands = QueryParser.Parse(query);
            commands.SelectMany(x => x.MatchedArguments).OfType<ParameterMatch>().ToList().ForEach(x => x.ResolveStorageType(document));

            var pipe = new List<QueryItem>();
            pipe.AddRange(VisibleInViewFilter.Create(commands, document));
            pipe.AddRange(ElementTypeFilter.Create(commands));
            pipe.AddRange(ElementIdFilter.Create(commands));
            pipe.AddRange(ClassFilter.Create(commands));
            pipe.AddRange(CategoryFilter.Create(commands));
            pipe.AddRange(StructuralTypeFilter.Create(commands));
            pipe.AddRange(LevelFilter.Create(commands));
            pipe.AddRange(RoomFilter.Create(commands, document));
            pipe.AddRange(RuleFilter.Create(commands, document));
            pipe.AddRange(ParameterFilter.Create(commands));

            var collector = new FilteredElementCollector(document);
            var collectorSyntax = "new FilteredElementCollector(document)";

            foreach (var filter in pipe)
            {
                if (filter.Filter != null)
                {
                    collector.WherePasses(filter.Filter);
                    collectorSyntax += Environment.NewLine + "    " + filter.CollectorSyntax;
                }
            }
            collectorSyntax += Environment.NewLine + "    .ToElements();";

            return new(collector, collectorSyntax, commands);
        }  
   

        public record Result(FilteredElementCollector Collector, string CollectorSyntax, IList<Command> Commands);
    }
}