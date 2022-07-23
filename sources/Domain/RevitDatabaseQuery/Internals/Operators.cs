using System.Linq;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Internals
{
    internal enum OperatorType { None, Equals, Greater, GreaterOrEqual, Less, LessOrEqual, HasNoValue, HasValue, NotEquals }


    internal static class Operators
    {
        private static readonly Operator None = new(OperatorType.None, null);
        private static readonly Operator[] operators = new[]
        {
            new Operator(OperatorType.NotEquals, "!="),
            new Operator(OperatorType.NotEquals, "<>"),
            new Operator(OperatorType.GreaterOrEqual, ">="),
            new Operator(OperatorType.LessOrEqual, "<="),
            new Operator(OperatorType.HasNoValue, "??"),
            new Operator(OperatorType.HasValue, "!!"),
            new Operator(OperatorType.Equals, "="),
            new Operator(OperatorType.Greater, ">"),            
            new Operator(OperatorType.Less, "<"), 
        };
        private static readonly char[] usedSymbols = new char[0];


        static Operators()
        {
            usedSymbols = operators.SelectMany(x => x.Symbol).Distinct().ToArray();
        }


        public static bool DoesContainAnyValidOperator(string text)
        {            
            return GetOperator(text) != None;
        }
        public static Operator GetOperator(string text)
        {
            foreach (var op in operators)
            {
                if (op.Type != OperatorType.None)
                {
                    if (text.IndexOf(op.Symbol) >= 0)
                    {
                        return op;
                    }
                }
            }
            return None;
        }
        public static OperatorWithArgument Parse(string argument)
        {
            var @operator = GetOperator(argument);

            if (@operator.Type != OperatorType.None)
            {
                argument = argument.Substring(argument.IndexOf(@operator.Symbol) + @operator.Symbol.Length);
            }

            int.TryParse(argument, out int intArg);
            double.TryParse(argument, out double doubleArg);

            var op = new OperatorWithArgument()
            {
                Type = @operator.Type,
                Symbol = @operator.Symbol,
                ArgumentAsString = argument.Trim(),
                ArgumentAsDouble = doubleArg,
                ArgumentAsInt = intArg,  
            };

            return op;
        }
    }


    internal class Operator
    {
        public OperatorType Type { get; init; } = OperatorType.None;
        public string Symbol { get; init; } = null;


        public Operator(OperatorType type = OperatorType.None, string symbol = null)
        {
            Type = type;
            Symbol = symbol;
        }
    }


    internal class OperatorWithArgument : Operator
    {        
        public string ArgumentAsString { get; init; } = "";
        public double ArgumentAsDouble { get; init; } = double.NaN;
        public int ArgumentAsInt { get; init; } = 0;
    }
}