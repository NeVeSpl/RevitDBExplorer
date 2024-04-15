using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class ParameterFilter : Filter
    {
        private readonly ParameterArgument parameterMatch;
        private readonly FilterRule rule;
        private readonly OperatorWithArgument @operator;


        public ParameterFilter(ParameterArgument parameterMatch, FilterRule rule, OperatorWithArgument @operator)
        {
            this.parameterMatch = parameterMatch;
            this.rule = rule;
            this.@operator = @operator;
            var argument = @operator.Evaluate(parameterMatch.DataType);

            var argForSyntax = $"\"{argument.String.Trim('*', '%')}\"";

            if (parameterMatch.StorageType == StorageType.Integer )
            {
                argForSyntax = argument.Int.ToString();
            }
            if (parameterMatch.StorageType == StorageType.ElementId && argument.IsArgumentInt)
            {
                argForSyntax = $"new ElementId({argument.Int})";
            }
            if (parameterMatch.StorageType == StorageType.Double)
            {
                argForSyntax = argument.Double.ToString(CultureInfo.InvariantCulture);
            }

            string parameterName = $"new ElementId({(parameterMatch.IsBuiltInParameter ? parameterMatch.Name : parameterMatch.Value)})";
            string ruleName = "";
            string secondArg = $", {argForSyntax}";
            string thirdArg = $"";

            if (rule is FilterNumericValueRule numericValueRule)
            {
                var evaluator = numericValueRule.GetEvaluator();
                ruleName = evaluator.GetType().Name.Substring("FilterNumeric".Length);
            }
            if (rule is FilterDoubleRule)
            {
                double epsilon = 1e-6;
                thirdArg = $", {epsilon}";
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

            FilterSyntax = $"new ElementParameterFilter(ParameterFilterRuleFactory.Create{ruleName}Rule({parameterName}{secondArg}{thirdArg}))";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var parameterCmds = commands.OfType<ParameterCmd>().ToList();
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

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementParameterFilter(rule, false);
        }

        private static QueryItem Create(ICommand command)
        {
            var arguments = command.Arguments.OfType<ParameterArgument>().ToList();
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

        private static IEnumerable<(FilterRule, ParameterArgument, OperatorWithArgument)> CreateFilterRule(ParameterArgument parameterMatch, OperatorWithArgument @operator)
        {
            var parameter = parameterMatch.Value;
            var storageType = parameterMatch.StorageType;
            var argument = @operator.Evaluate(parameterMatch.DataType);  

            FilterRule rule = null;

            if (storageType == StorageType.Integer)
            {
                rule = @operator.Type switch
                {
                    OperatorType.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameter, argument.Int),
                    OperatorType.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, argument.Int),
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argument.Int),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argument.Int),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argument.Int),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argument.Int),
                    _ => null
                };
            }


            if (storageType == StorageType.String || storageType == StorageType.None|| (storageType == (StorageType.ElementId) && !argument.IsArgumentInt))
            {
                var argAsString = argument.String;
                bool startsWithWildcard = argAsString.StartsWith("*") || argAsString.StartsWith("%");
                bool endsWithWildcard = argAsString.EndsWith("*") || argAsString.EndsWith("%");
                var serchTerm = argAsString.Trim('*', '%');

                rule = @operator.Type switch
                {
#if R2022_MAX
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argAsString, false),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argAsString, false),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argAsString, false),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argAsString, false),
#endif
#if R2023_MIN
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argAsString),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argAsString),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argAsString),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argAsString),
#endif
                    _ => null
                };

                if (@operator.Type == OperatorType.Equals)
                {
#if R2023_MIN
                    if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateContainsRule(parameter, serchTerm);
                    if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEndsWithRule(parameter, serchTerm);
                    if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateBeginsWithRule(parameter, serchTerm);
                    if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEqualsRule(parameter, serchTerm);
#endif
#if R2022_MAX
                    if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateContainsRule(parameter, serchTerm, false);
                    if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEndsWithRule(parameter, serchTerm, false);
                    if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateBeginsWithRule(parameter, serchTerm, false);
                    if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateEqualsRule(parameter, serchTerm, false);
#endif
                }
                if (@operator.Type == OperatorType.NotEquals)
                {
#if R2023_MIN
                    if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotContainsRule(parameter, serchTerm);
                    if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEndsWithRule(parameter, serchTerm);
                    if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotBeginsWithRule(parameter, serchTerm);
                    if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, serchTerm);
#endif
#if R2022_MAX
                    if (startsWithWildcard == true && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotContainsRule(parameter, serchTerm, false);
                    if (startsWithWildcard == true && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEndsWithRule(parameter, serchTerm, false);
                    if (startsWithWildcard == false && endsWithWildcard == true) rule = ParameterFilterRuleFactory.CreateNotBeginsWithRule(parameter, serchTerm, false);
                    if (startsWithWildcard == false && endsWithWildcard == false) rule = ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, serchTerm, false);
#endif
                }

            }

            
            if (storageType == (StorageType.Double))
            {
                double epsilon = 1e-5;
                rule = @operator.Type switch
                {
                    OperatorType.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameter, argument.Double, epsilon),
                    OperatorType.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, argument.Double, epsilon),
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argument.Double, epsilon),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argument.Double, epsilon),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argument.Double, epsilon),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argument.Double, epsilon),
                    _ => null
                };
            }


            if (storageType == (StorageType.ElementId) && argument.IsArgumentInt)
            {              
                rule = @operator.Type switch
                {
                    OperatorType.Equals => ParameterFilterRuleFactory.CreateEqualsRule(parameter, argument.ElementId),
                    OperatorType.NotEquals => ParameterFilterRuleFactory.CreateNotEqualsRule(parameter, argument.ElementId),
                    OperatorType.Greater => ParameterFilterRuleFactory.CreateGreaterRule(parameter, argument.ElementId),
                    OperatorType.GreaterOrEqual => ParameterFilterRuleFactory.CreateGreaterOrEqualRule(parameter, argument.ElementId),
                    OperatorType.Less => ParameterFilterRuleFactory.CreateLessRule(parameter, argument.ElementId),
                    OperatorType.LessOrEqual => ParameterFilterRuleFactory.CreateLessOrEqualRule(parameter, argument.ElementId),
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