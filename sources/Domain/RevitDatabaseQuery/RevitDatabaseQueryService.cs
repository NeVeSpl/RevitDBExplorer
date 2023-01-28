using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
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


        public static Result ParseAndExecute(Document document, string query)
        {
            if (document is null) return new Result(null, new List<Command>(), new SourceOfObjects());  


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

            return new Result(collectorSyntax, commands, new SourceOfObjects(new RemoteExecutor(collector, document)));
        }

        public record Result(string GeneratedCSharpSyntax, IList<Command> Commands, SourceOfObjects SourceOfObjects);

        public class RemoteExecutor : IAmSourceOfEverything
        {
            private readonly FilteredElementCollector collector;
            private readonly Document document;


            public RemoteExecutor(FilteredElementCollector collector, Document document)
            {              
                this.collector = collector;
                this.document = document;
            }


            public IEnumerable<SnoopableObject> Snoop(UIApplication app)
            {
                if (document == null) return null;
                var snoopableObjects = collector.ToElements().Select(x => new SnoopableObject(document, x));
                return snoopableObjects;
            }
        }








        public static Result ParseAndBuild(Document document, string query)
        {
            var commandsText = RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.QueryParser.Parse(query);
            var commands = commandsText.SelectMany(x => RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.CommandParser.Parse(x)).ToArray();


            return null;
        }
    }
}