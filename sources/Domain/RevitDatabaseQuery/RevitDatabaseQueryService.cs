using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;
using VisibleInViewFilter = RevitDBExplorer.Domain.RevitDatabaseQuery.Filters.VisibleInViewFilter;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal static class RevitDatabaseQueryService
    {
        public static void Init()
        {
            CommandParser.Init();          
        }


        public static Result ParseAndExecute(Document document, string query)
        {
            if (document is null) return new Result(null, new List<ICommand>(), new SourceOfObjects() { Title = "Query" });

            CommandParser.LoadDocumentSpecificData(document);
            var commands = QueryParser.Parse(query);
            commands.SelectMany(x => x.Arguments).OfType<ParameterArgument>().ToList().ForEach(x => x.ResolveStorageType(document));

            var pipe = new List<QueryItem>();
            pipe.AddRange(VisibleInViewFilter.Create(commands, document));
            pipe.AddRange(ElementTypeFilter.Create(commands));
            pipe.AddRange(ElementIdFilter.Create(commands));
            pipe.AddRange(ClassFilter.Create(commands));
            pipe.AddRange(CategoryFilter.Create(commands));
            pipe.AddRange(StructuralTypeFilter.Create(commands));
            pipe.AddRange(LevelFilter.Create(commands));
            pipe.AddRange(OwnerViewFilter.Create(commands, document));
            pipe.AddRange(RoomFilter.Create(commands, document));
            pipe.AddRange(RuleFilter.Create(commands, document));
            pipe.AddRange(ParameterFilter.Create(commands));
            pipe.AddRange(Filters.WorksetFilter.Create(commands));

            string collectorSyntax = "";
            CollectorQueryExecutor queryExecutor = null;

            if (pipe.Any())
            { 
                var collector = new FilteredElementCollector(document);
                collectorSyntax = "new FilteredElementCollector(document)";

                foreach (var filter in pipe)
                {
                    if (filter.Filter != null)
                    {
                        collector.WherePasses(filter.Filter);
                        collectorSyntax += Environment.NewLine + "    " + filter.CollectorSyntax;
                    }
                }
                collectorSyntax += Environment.NewLine + "    .ToElements()";

                queryExecutor = new CollectorQueryExecutor(collector, document);
            }           

            return new Result(collectorSyntax, commands, new SourceOfObjects(queryExecutor) { Title ="Query" });
        }

        public record Result(string GeneratedCSharpSyntax, IList<ICommand> Commands, SourceOfObjects SourceOfObjects);

        private class CollectorQueryExecutor : IAmSourceOfEverything
        {
            private readonly FilteredElementCollector collector;
            private readonly Document document;
           

            public CollectorQueryExecutor(FilteredElementCollector collector, Document document)
            {
                this.collector = collector;
                this.document = document;                
            }


            public IEnumerable<SnoopableObject> Snoop(UIApplication app)
            {
                var snoopableObjects = collector.ToElements().Select(x => new SnoopableObject(document, x));
                return snoopableObjects;
            }
        }
    }
}