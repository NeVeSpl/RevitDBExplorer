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
            var t7 = OfParameter(t6, commands, document);

            return new(t7.Collector, t7.CollectorSyntax, commands);
        }


        private static Token CreateCollector(Document document, List<Command> commands)
        {
            if (commands.Any(x => x.Type == CmdType.ActiveView))
            {
                var c = new FilteredElementCollector(document, document.ActiveView.Id);
                var s = "new FilteredElementCollector(document, document.ActiveView.Id)";
                return new Token(c, s);
            }
            else
            {
                var c = new FilteredElementCollector(document);
                var s = "new FilteredElementCollector(document)";
                return new Token(c, s);
            }
        }
        private static Token WhereElementIsElementTypeOrNot(Token token, List<Command> commands)
        {
            var isElementTypePresent = commands.Any(x => x.Type == CmdType.ElementType);
            var isNotElementTypePresent = commands.Any(x => x.Type == CmdType.NotElementType);

            if (isElementTypePresent == true && isNotElementTypePresent == false)
            {
                var c = token.Collector.WhereElementIsElementType();
                var s = token.CollectorSyntax + ".WhereElementIsElementType()";
                return new Token(c, s);
            }
            if (isElementTypePresent == false && isNotElementTypePresent == true)
            {
                var c = token.Collector.WhereElementIsNotElementType();
                var s = token.CollectorSyntax + ".WhereElementIsNotElementType()";
                return new Token(c, s);
            }
            if (isElementTypePresent == true && isNotElementTypePresent == true)
            {
                var c = token.Collector.WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)));
                var s = token.CollectorSyntax + ".WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)))";
                return new Token(c, s);
            }

            return token;
        }
        private static Token WherePassesElementIdSetFilter(Token token, List<Command> commands)
        {
            var ids = commands.SelectMany(x => x.Arguments).Where(x => x.IsElementId).OfType<LookupResult<ElementId>>().ToList();
            if (ids.Any())
            {
                var filter = new ElementIdSetFilter(ids.Select(x => x.Value).ToList());
                var c = token.Collector.WherePasses(filter);
                var s = token.CollectorSyntax + ".WherePasses(new ElementIdSetFilter(new [] {" + String.Join(", ", ids.Select(x => x.Name)) + "}))";
                return new Token(c, s);
            }
            return token;
        }
        private static Token OfClass(Token token, List<Command> commands)
        {
            var types = commands.SelectMany(x => x.Arguments).Where(x => x.IsClass).OfType<LookupResult<Type>>().ToList();
            if (types.Count == 1)
            {
                var type = types.First();
                var c = token.Collector.OfClass(type.Value);
                var s = token.CollectorSyntax + $".OfClass({type.Name})";
                return new Token(c, s);
            }
            if (types.Count >= 1)
            {
                var filter = new ElementMulticlassFilter(types.Select(x => x.Value).ToList());
                var c = token.Collector.WherePasses(filter);
                var s = token.CollectorSyntax + ".WherePasses(new ElementMulticlassFilter(new [] {" + String.Join(", ", types.Select(x => x.Name)) + "}))";
                return new Token(c, s);
            }
            return token;
        }
        private static Token OfCategory(Token token, List<Command> commands)
        {
            var categories = commands.SelectMany(x => x.Arguments).Where(x => x.IsCategory).OfType<LookupResult<BuiltInCategory>>().ToList();
            if (categories.Count == 1)
            {
                var category = categories.First();
                var c = token.Collector.OfCategory(category.Value);
                var s = token.CollectorSyntax + $".OfCategory({category.Name})";
                return new Token(c, s);
            }
            if (categories.Count >= 1)
            {
                var filter = new ElementMulticategoryFilter(categories.Select(x => x.Value).ToList());
                var c = token.Collector.WherePasses(filter);
                var s = token.CollectorSyntax + ".WherePasses(new ElementMulticategoryFilter(new [] {" + String.Join(", ", categories.Select(x => x.Name)) + "}))";
                return new Token(c, s);
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
        private static Token OfName(Token token, List<Command> commands)
        {
            var names = commands.Where(x => x.Type == CmdType.NameParam).Select(x => x.Argument).Where(x => !String.IsNullOrEmpty(x)).ToList();
            if (names.Any())
            {
                var rules = new List<FilterRule>();
                foreach (var name in names)
                {
                    foreach (var parameter in NameLikeParameters)
                    {
#if R2021 || R2022
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
                return new Token(c, s);
            }
            return token;
        }
        private static Token OfParameter(Token token, List<Command> commands, Document document)
        {
            var parameters = commands.Where(x => x.Type == CmdType.Parameter).ToList();

            if (parameters.Any())
            {
                var filtersForCmds = new List<ElementFilter>();
                foreach (var parameterCmd in parameters)
                {
                    var rules = new List<FilterRule>();
                    foreach (var argument in parameterCmd.Arguments.OfType<LookupResult<ForgeTypeId>>())
                    {
                        var forgeId =  argument.Value;
                        var builtInParameter = ParameterUtils.GetBuiltInParameter(forgeId);
                        var parameter = new ElementId(builtInParameter);
                        var storageType = document.get_TypeOfStorage(builtInParameter);
                        FilterRule rule = null;
                        if (storageType == StorageType.Integer)
                        {
                            int.TryParse(parameterCmd.OperatorArgumentAsString, out int intArg);
                            rule = parameterCmd.Operator switch
                            {
                                Operator.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameter, intArg),
                                Operator.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, intArg),
                                Operator.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, intArg),
                                Operator.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, intArg),
                                Operator.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, intArg),
                                Operator.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, intArg),
                                _ => null
                            };

                        }
                        if (storageType == StorageType.String)
                        {
                            rule = parameterCmd.Operator switch
                            {
#if R2022
                                Operator.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, parameterCmd.OperatorArgumentAsString, false),
                                Operator.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, parameterCmd.OperatorArgumentAsString, false),
                                Operator.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, parameterCmd.OperatorArgumentAsString, false),
                                Operator.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, parameterCmd.OperatorArgumentAsString, false),
#endif
#if R2023
                                Operator.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, parameterCmd.OperatorArgumentAsString),
                                Operator.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, parameterCmd.OperatorArgumentAsString),
                                Operator.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, parameterCmd.OperatorArgumentAsString),
                                Operator.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, parameterCmd.OperatorArgumentAsString),
#endif
                                _ => null
                            };
                            var serchTerm = parameterCmd.OperatorArgumentAsString.Trim();
                            bool startsWithWildcard = serchTerm.StartsWith("*") || serchTerm.StartsWith("%");
                            bool endsWithWildcard = serchTerm.EndsWith("*") || serchTerm.EndsWith("%");
                            serchTerm = serchTerm.Trim('*', '%');

                            if (parameterCmd.Operator == Operator.Equals)
                            {
#if R2023
                                if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateContainsRule(parameter, serchTerm);
                                if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEndsWithRule(parameter, serchTerm);
                                if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateBeginsWithRule(parameter, serchTerm);
                                if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEqualsRule(parameter, serchTerm);
#endif
#if R2022
                                if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateContainsRule(parameter, serchTerm, false);
                                if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEndsWithRule(parameter, serchTerm, false);
                                if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateBeginsWithRule(parameter, serchTerm, false);
                                if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEqualsRule(parameter, serchTerm, false);
#endif
                            }
                            if (parameterCmd.Operator == Operator.NotEquals)
                            {
#if R2023
                                if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotContainsRule(parameter, serchTerm);
                                if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEndsWithRule(parameter, serchTerm);
                                if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotBeginsWithRule(parameter, serchTerm);
                                if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, serchTerm);
#endif
#if R2022
                                if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotContainsRule(parameter, serchTerm, false);
                                if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEndsWithRule(parameter, serchTerm, false);
                                if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotBeginsWithRule(parameter, serchTerm, false);
                                if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, serchTerm, false);
#endif
                            }

                        }
                        double epsilon = 1e-6;
                        if (storageType == (StorageType.Double))
                        {
                            double.TryParse(parameterCmd.OperatorArgumentAsString, out double doubleArg);
                            rule = parameterCmd.Operator switch
                            {
                                Operator.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameter, doubleArg, epsilon),
                                Operator.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, doubleArg, epsilon),
                                Operator.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, doubleArg, epsilon),
                                Operator.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, doubleArg, epsilon),
                                Operator.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, doubleArg, epsilon),
                                Operator.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, doubleArg, epsilon),
                                _ => null
                            };

                        }
                        rule = parameterCmd.Operator switch
                        {
                            Operator.HasValue => ParameterFilterRuleFactory.CreateHasValueParameterRule(parameter),
                            Operator.HasNoValue => ParameterFilterRuleFactory.CreateHasNoValueParameterRule(parameter),
                            _ => rule
                        };
                        if (rule != null)
                        {
                            rules.Add(rule);
                        }
                     }
                    var or = new LogicalOrFilter(rules.Select(x => new ElementParameterFilter(x, false)).ToList<ElementFilter>());
                    filtersForCmds.Add(or);
                }
                var finalFilter = new LogicalAndFilter(filtersForCmds);
                var c = token.Collector.WherePasses(finalFilter);
                var s = token.CollectorSyntax;
                return new Token(c, s);
            }

            return token;
        }



        private record Token(FilteredElementCollector Collector, string CollectorSyntax);
        public record Result(FilteredElementCollector Collector, string CollectorSyntax, IList<Command> Commands);
    }
}