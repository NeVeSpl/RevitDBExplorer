using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

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
            var commands = QueryParser.Parse(query);

            var t1 = CreateCollector(document, commands);
            var t2 = WhereElementIsElementTypeOrNot(t1, commands);
            var t3 = WherePassesElementIdSetFilter(t2, commands);
            var t4 = OfClass(t3, commands);
            var t5 = OfCategory(t4, commands);
            var t6 = OfName(t5, commands);

            return t6;
        }


        private static Result CreateCollector(Document document, List<Command> commands)
        {
            if (commands.Any(x => x.Type == CmdType.ActiveView))
            {
                var c = new FilteredElementCollector(document, document.ActiveView.Id);
                var s = "new FilteredElementCollector(document, document.ActiveView.Id)";
                return new Result(c, s);
            }
            else
            {
                var c = new FilteredElementCollector(document);
                var s = "new FilteredElementCollector(document)";
                return new Result(c, s);
            }
        }
        private static Result WhereElementIsElementTypeOrNot(Result token, List<Command> commands)
        {
            var isElementTypePresent = commands.Any(x => x.Type == CmdType.ElementType);
            var isNotElementTypePresent = commands.Any(x => x.Type == CmdType.NotElementType);

            if (isElementTypePresent == true && isNotElementTypePresent == false)
            {
                var c = token.Collector.WhereElementIsElementType();
                var s = token.CollectorSyntax + ".WhereElementIsElementType()";
                return new Result(c, s);
            }
            if (isElementTypePresent == false && isNotElementTypePresent == true)
            {
                var c = token.Collector.WhereElementIsNotElementType();
                var s = token.CollectorSyntax + ".WhereElementIsNotElementType()";
                return new Result(c, s);
            }
            if (isElementTypePresent == true && isNotElementTypePresent == true)
            {
                var c = token.Collector.WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)));
                var s = token.CollectorSyntax + ".WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)))";
                return new Result(c, s);
            }

            return token;
        }
        private static Result WherePassesElementIdSetFilter(Result token, List<Command> commands)
        {
            var ids = commands.SelectMany(x => x.Arguments).Where(x => x.IsElementId).OfType<LookupResult<ElementId>>().ToList();
            if (ids.Any())
            {
                var filter = new ElementIdSetFilter(ids.Select(x => x.Value).ToList());
                var c = token.Collector.WherePasses(filter);
                var s = token.CollectorSyntax + ".WherePasses(new ElementIdSetFilter(new [] {" + String.Join(", ", ids.Select(x => x.Name)) + "}))";
                return new Result(c, s);
            }
            return token;
        }
        private static Result OfClass(Result token, List<Command> commands)
        {
            var types = commands.SelectMany(x => x.Arguments).Where(x => x.IsClass).OfType<LookupResult<Type>>().ToList();
            if (types.Count == 1)
            {
                var type = types.First();
                var c = token.Collector.OfClass(type.Value);
                var s = token.CollectorSyntax + $".OfClass({type.Name})";
                return new Result(c, s);
            }
            if (types.Count >= 1)
            {
                var filter = new ElementMulticlassFilter(types.Select(x => x.Value).ToList());
                var c = token.Collector.WherePasses(filter);
                var s = token.CollectorSyntax + ".WherePasses(new ElementMulticlassFilter(new [] {" + String.Join(", ", types.Select(x => x.Name)) + "}))";
                return new Result(c, s);
            }
            return token;
        }
        private static Result OfCategory(Result token, List<Command> commands)
        {
            var categories = commands.SelectMany(x => x.Arguments).Where(x => x.IsCategory).OfType<LookupResult<BuiltInCategory>>().ToList();
            if (categories.Count == 1)
            {
                var category = categories.First();
                var c = token.Collector.OfCategory(category.Value);
                var s = token.CollectorSyntax + $".OfCategory({category.Name})";
                return new Result(c, s);
            }
            if (categories.Count >= 1)
            {
                var filter = new ElementMulticategoryFilter(categories.Select(x => x.Value).ToList());
                var c = token.Collector.WherePasses(filter);
                var s = token.CollectorSyntax + ".WherePasses(new ElementMulticategoryFilter(new [] {" + String.Join(", ", categories.Select(x => x.Name)) + "}))";
                return new Result(c, s);
            }
            return token;
        }

        private static readonly List<BuiltInParameter> NameLikeParameters = new List<BuiltInParameter>()
        {
            BuiltInParameter.ALL_MODEL_TYPE_NAME,
            BuiltInParameter.ALL_MODEL_MARK,
            BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM,
            BuiltInParameter.DATUM_TEXT
        };
        private static Result OfName(Result token, List<Command> commands)
        {
            var names = commands.Where(x => x.Type == CmdType.NameParam).Select(x => x.Argument).Where(x => !String.IsNullOrEmpty(x)).ToList();
            if (names.Any())
            {
                var rules = new List<FilterRule>();
                foreach (var name in names)
                {
                    foreach (var parameter in NameLikeParameters)
                    {
#if R2022
                        rules.Add(ParameterFilterRuleFactory.CreateContainsRule(new ElementId(parameter), name, false));
#endif
#if R2023
                        rules.Add(ParameterFilterRuleFactory.CreateContainsRule(new ElementId(parameter), name));
#endif
                    }
                }

                var or = new LogicalOrFilter(rules.Select(x => new ElementParameterFilter(x, false)).ToList<ElementFilter>());
                var c = token.Collector.WherePasses(or);
                var s = token.CollectorSyntax + $".WherePasses(Name = " + String.Join(", ", names.Select(x => $"{x}")) + ")";
                return new Result(c, s);
            }
            return token;
        }


        public record Result(FilteredElementCollector Collector, string CollectorSyntax);
    }
}