using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class ParameterFilter : Filter
    {
        private readonly ParameterMatch parameterMatch;
        private readonly FilterRule rule;
        private readonly OperatorWithArgument @operator;


        public ParameterFilter(ParameterMatch parameterMatch, FilterRule rule, OperatorWithArgument @operator)
        {
            this.parameterMatch = parameterMatch;
            this.rule = rule;
            this.@operator = @operator;

            Filter = new ElementParameterFilter(rule, false);

            var argForSyntax = $"\"{@operator.ArgumentAsString.Trim('*', '%')}\"";

            if (parameterMatch.StorageType == StorageType.Integer )
            {
                argForSyntax = @operator.ArgumentAsInt.ToString();
            }
            if (parameterMatch.StorageType == StorageType.ElementId && @operator.IsArgumentInt)
            {
                argForSyntax = $"new ElementId({@operator.ArgumentAsInt})";
            }
            if (parameterMatch.StorageType == StorageType.Double)
            {
                argForSyntax = @operator.ArgumentAsDouble.ToString();
            }

            string parameterName = $"new ElementId({(parameterMatch.IsBuiltInParameter ? parameterMatch.Name : parameterMatch.Value)})";
            string ruleName = "";
            string secondArg = $", {argForSyntax}";

            if (rule is FilterNumericValueRule numericValueRule)
            {
                var evaluator = numericValueRule.GetEvaluator();
                ruleName = evaluator.GetType().Name.Substring("FilterNumeric".Length);
            }
            if (rule is FilterStringRule stringRule)
            {
                var evaluator = stringRule.GetEvaluator();
                ruleName = evaluator.GetType().Name.Substring("FilterString".Length);
            }
            if (rule is HasNoValueFilterRule)
            {
                ruleName = "HasNoValueParameter";
                secondArg = "";
            }
            if (rule is HasValueFilterRule)
            {
                ruleName = "HasValueParameter";
                secondArg = "";
            }

            FilterSyntax = $"new ElementParameterFilter(ParameterFilterRuleFactory.Create{ruleName}Rule({parameterName}{secondArg}))";
        }


        public static IEnumerable<QueryItem> Create(List<Command> commands)
        {
            var parameterCmds = commands.Where(x => x.Type == CmdType.Parameter).ToList();
            var filters = parameterCmds.Select(x => Create(x)).Where(x => x != null).ToList();
            if (filters.Count == 1)
            {                
                 yield return filters.First();                
            }
            if (filters.Count > 1)
            { 
                yield return new Group(filters, LogicalOperator.And);                
            }
        }

        private static QueryItem Create(Command command)
        {
            var arguments = command.MatchedArguments.OfType<ParameterMatch>().ToList();
            var rules = arguments.SelectMany(x => CreateFilterRule(x, command.Operator)).ToList();
            var filters = rules.Select(x => new ParameterFilter(x.Item2, x.Item1, x.Item3)).ToList();
            if (filters.Count == 1)
            {              
                return filters.First();
            }
            if (filters.Count > 1)
            {
                return new Group(filters, LogicalOperator.Or);
            }
            return null;
        }

        private static IEnumerable<(FilterRule, ParameterMatch, OperatorWithArgument)> CreateFilterRule(ParameterMatch parameterMatch, OperatorWithArgument @operator)
        {
            var parameter = parameterMatch.Value;
            var storageType = parameterMatch.StorageType;
            var argAsInt = @operator.ArgumentAsInt;
            var argAsDouble = @operator.ArgumentAsDouble;
            var argAsString = @operator.ArgumentAsString;
            bool startsWithWildcard = argAsString.StartsWith("*") || argAsString.StartsWith("%");
            bool endsWithWildcard = argAsString.EndsWith("*") || argAsString.EndsWith("%");
            var serchTerm = argAsString.Trim('*', '%');          

            FilterRule rule = null;

            if (storageType == StorageType.Integer)
            {
                rule = @operator.Type switch
                {
                    OperatorType.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameter, argAsInt),
                    OperatorType.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, argAsInt),
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argAsInt),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argAsInt),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argAsInt),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argAsInt),
                    _ => null
                };
            }

            if (storageType == StorageType.String || storageType == StorageType.None|| (storageType == (StorageType.ElementId) && !@operator.IsArgumentInt) || (storageType == (StorageType.Double) && !@operator.IsArgumentDouble))
            {
                rule = @operator.Type switch
                {
#if R2022e
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argAsString, false),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argAsString, false),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argAsString, false),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argAsString, false),
#endif
#if R2023b
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argAsString),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argAsString),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argAsString),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argAsString),
#endif
                    _ => null
                };

                if (@operator.Type == OperatorType.Equals)
                {
#if R2023b
                    if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateContainsRule(parameter, serchTerm);
                    if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEndsWithRule(parameter, serchTerm);
                    if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateBeginsWithRule(parameter, serchTerm);
                    if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEqualsRule(parameter, serchTerm);
#endif
#if R2022e
                    if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateContainsRule(parameter, serchTerm, false);
                    if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEndsWithRule(parameter, serchTerm, false);
                    if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateBeginsWithRule(parameter, serchTerm, false);
                    if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEqualsRule(parameter, serchTerm, false);
#endif
                }
                if (@operator.Type == OperatorType.NotEquals)
                {
#if R2023b
                    if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotContainsRule(parameter, serchTerm);
                    if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEndsWithRule(parameter, serchTerm);
                    if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotBeginsWithRule(parameter, serchTerm);
                    if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, serchTerm);
#endif
#if R2022e
                    if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotContainsRule(parameter, serchTerm, false);
                    if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEndsWithRule(parameter, serchTerm, false);
                    if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotBeginsWithRule(parameter, serchTerm, false);
                    if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, serchTerm, false);
#endif
                }

            }

            double epsilon = 1e-6;
            if (storageType == (StorageType.Double) && @operator.IsArgumentDouble)
            {
                rule = @operator.Type switch
                {
                    OperatorType.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameter, argAsDouble, epsilon),
                    OperatorType.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, argAsDouble, epsilon),
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argAsDouble, epsilon),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argAsDouble, epsilon),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argAsDouble, epsilon),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argAsDouble, epsilon),
                    _ => null
                };

            }

            if (storageType == (StorageType.ElementId) && @operator.IsArgumentInt)
            {
                var argAsId = ElementIdFactory.Create(@operator.ArgumentAsInt);
                rule = @operator.Type switch
                {
                    OperatorType.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameter, argAsId),
                    OperatorType.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, argAsId),
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argAsId),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argAsId),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argAsId),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argAsId),
                    _ => null
                };
            }

            rule = @operator.Type switch
            {
                OperatorType.HasValue => ParameterFilterRuleFactory.CreateHasValueParameterRule(parameter),
                OperatorType.HasNoValue => ParameterFilterRuleFactory.CreateHasNoValueParameterRule(parameter),
                _ => rule
            };
            if (rule != null)
            {
                yield return (rule, parameterMatch, @operator);
            }
            if (@operator.Type == OperatorType.Exists)
            {
                yield return (ParameterFilterRuleFactory.CreateHasValueParameterRule(parameter), parameterMatch, @operator);
                yield return (ParameterFilterRuleFactory.CreateHasNoValueParameterRule(parameter), parameterMatch, @operator);              
            }
        }
    }
}