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

            string collectorSyntax = "";
            QueryPipeExecutor queryExecutor = null;

            bool useRevitSelection = commands.OfType<SelectionCmd>().Any();

            if (pipe.Any() || useRevitSelection)
            {  
                collectorSyntax = useRevitSelection ? "new FilteredElementCollector(document, app.ActiveUIDocument.Selection.GetElementIds())" : "new FilteredElementCollector(document)";

                foreach (var filter in pipe)
                {
                   collectorSyntax += Environment.NewLine + "    " + filter.CollectorSyntax;                    
                }
                collectorSyntax += Environment.NewLine + "    .ToElements()";

                queryExecutor = new QueryPipeExecutor(pipe, useRevitSelection);
            }           

            return new Result(collectorSyntax, commands, new SourceOfObjects(queryExecutor) { Title ="Query" });
        }

        public record Result(string GeneratedCSharpSyntax, IList<ICommand> Commands, SourceOfObjects SourceOfObjects);

        private class QueryPipeExecutor : IAmSourceOfEverything
        {
            private readonly IReadOnlyList<QueryItem> pipe;
            private readonly bool useRevitSelection;

            public QueryPipeExecutor(IReadOnlyList<QueryItem> pipe, bool useSelection)
            {
                this.pipe = pipe;
                this.useRevitSelection = useSelection;
            }


            public IEnumerable<SnoopableObject> Snoop(UIApplication app)
            {
                var document = app.ActiveUIDocument?.Document;
                if (document == null) return null;

                ICollection<ElementId> selectedIds = null;

                if (useRevitSelection)
                {
                    selectedIds = app.ActiveUIDocument.Selection.GetElementIds();
                    if (selectedIds.Any() == false)
                    {
                        return null;
                    }
                }

                var collector = BuildCollector(document, selectedIds);
                var snoopableObjects = collector.ToElements().Select(x => new SnoopableObject(document, x));
                return snoopableObjects;
            }

            private FilteredElementCollector BuildCollector(Document document, ICollection<ElementId> selectedIds)
            {
                var collector = selectedIds != null ? new FilteredElementCollector(document, selectedIds) : new FilteredElementCollector(document);              

                foreach (var filter in pipe)
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