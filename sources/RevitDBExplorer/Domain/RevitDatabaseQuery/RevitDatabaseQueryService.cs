using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Providers;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Providers.Internals;
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
            if (document is null) return new Result(null, new List<ICommand>(), new SourceOfObjects() { Info = new InfoAboutSource("Query") });

            CommandParser.LoadDocumentSpecificData(document);
            var commands = QueryParser.Parse(query);
            commands.SelectMany(x => x.Arguments).OfType<ParameterArgument>().ToList().ForEach(x => x.ResolveStorageType(document));

            var pipe = new List<QueryItem>();
            pipe.AddRange(VisibleInViewFilter.Create(commands));
            pipe.AddRange(ElementTypeFilter.Create(commands));
            pipe.AddRange(ElementIdFilter.Create(commands));
            pipe.AddRange(ClassFilter.Create(commands));
            pipe.AddRange(CategoryFilter.Create(commands));
            pipe.AddRange(StructuralTypeFilter.Create(commands));
            pipe.AddRange(LevelFilter.Create(commands));
            pipe.AddRange(OwnerViewFilter.Create(commands));
            pipe.AddRange(RoomFilter.Create(commands));
            pipe.AddRange(RuleFilter.Create(commands));
            pipe.AddRange(ParameterFilter.Create(commands));
            pipe.AddRange(Filters.WorksetFilter.Create(commands));

            string providerSyntax = "";
            string collectorSyntax = "";
            QueryPipeExecutor queryExecutor = null;

            var providers = new List<Provider>();
            providers.AddRange(SelectionProvider.Create(commands));
            providers.AddRange(UniqueIdProvider.Create(commands));

            if (pipe.Any())
            {
                var elementIdsSyntax = "";

                if (providers.Any())
                {
                    providerSyntax = "    var elementIds = new List<ElementId>();";

                    foreach (var provider in providers)
                    {
                        providerSyntax += Environment.NewLine + "    " + provider.Syntax;
                    }

                    providerSyntax += Environment.NewLine + Environment.NewLine;
                    elementIdsSyntax = ", elementIds";
                }

                collectorSyntax = $"    return new FilteredElementCollector(document{elementIdsSyntax})";

                foreach (var filter in pipe)
                {
                   collectorSyntax += Environment.NewLine + "    " + filter.CollectorSyntax;                    
                }
                collectorSyntax += Environment.NewLine + "    .ToElements();";

                queryExecutor = new QueryPipeExecutor(pipe, providers);
            }           

            return new Result(providerSyntax + collectorSyntax, commands, new SourceOfObjects(queryExecutor) { Info = new InfoAboutSource("Query") });
        }

        public record Result(string GeneratedCSharpSyntax, IList<ICommand> Commands, SourceOfObjects SourceOfObjects);

        private class QueryPipeExecutor : IAmSourceOfEverything
        {
            private readonly IReadOnlyList<QueryItem> filterPipe;
            private readonly IReadOnlyList<Provider> seedPipe;


            public QueryPipeExecutor(IReadOnlyList<QueryItem> filterPipe, IReadOnlyList<Provider> seedPipe)
            {
                this.filterPipe = filterPipe;
                this.seedPipe = seedPipe;
            }


            public IEnumerable<SnoopableObject> Snoop(UIApplication app)
            {
                var document = app.ActiveUIDocument?.Document;
                if (document == null) return null;

                ICollection<ElementId> elementIds = GatherInitialSeed(app.ActiveUIDocument).ToArray();

                if (seedPipe.Any() && elementIds.IsEmpty())
                {
                     return null;
                }

                var collector = BuildCollector(document, elementIds);
                var snoopableObjects = collector.ToElements().Select(x => new SnoopableObject(document, x));

                return snoopableObjects;
            }

            private IEnumerable<ElementId> GatherInitialSeed(UIDocument uiDocument)
            {
                foreach (var provider in seedPipe)
                {
                    foreach (var id in provider.GetIds(uiDocument))
                    {
                        yield return id;
                    }
                }
            }
            private FilteredElementCollector BuildCollector(Document document, ICollection<ElementId> elementIds)
            {
                FilteredElementCollector collector;
                if (elementIds?.Any() == true)
                {
                    collector = new FilteredElementCollector(document, elementIds);
                }
                else
                {
                    collector = new FilteredElementCollector(document);
                }

                foreach (var filter in filterPipe)
                {
                    var elementFilter = filter.CreateElementFilter(document);
                    if (elementFilter != null)
                    {
                        collector.WherePasses(elementFilter);                        
                    }
                }

                return collector;
            }
        }
    }
}