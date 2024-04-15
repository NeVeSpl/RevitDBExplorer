using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal enum OperatorType { None, Equals, Greater, GreaterOrEqual, Less, LessOrEqual, HasNoValue, HasValue, NotEquals, Exists }


    internal static class Operators
    {
        public static readonly Operator None = new(OperatorType.None, "");
        private static readonly Operator[] operators = new[]
        {
            new Operator(OperatorType.NotEquals, "!="),
            new Operator(OperatorType.NotEquals, "<>"),
            new Operator(OperatorType.GreaterOrEqual, ">="),
            new Operator(OperatorType.LessOrEqual, "<="),
            new Operator(OperatorType.HasNoValue, "??"),
            new Operator(OperatorType.HasValue, "!!"),
            new Operator(OperatorType.Exists, "?!"),
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
        private static Operator GetOperator(string text)
        {
            foreach (var op in operators)
            {                
                if (text.IndexOf(op.Symbol, System.StringComparison.Ordinal) >= 0)
                {
                    return op;
                }                
            }
            return None;
        }
        public static OperatorWithArgument Parse(string argument)
        {
            var @operator = GetOperator(argument);
            var opArgument = "";

            if (@operator.Type != OperatorType.None)
            {
                opArgument = argument.Substring(argument.IndexOf(@operator.Symbol) + @operator.Symbol.Length);
            }
            else
            {
                @operator = operators.FirstOrDefault(x => x.Type == OperatorType.Exists);
            }

            return new OperatorWithArgument(@operator, opArgument);
        }
        public static string GetLeftSideOfOperator(string text)
        {
            foreach (var op in operators)
            {
                var index = text.IndexOf(op.Symbol, System.StringComparison.Ordinal);
                if (index >= 0)
                {
                    return text.Substring(0, index);
                }
            }
            return text;
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

    internal class ArgumentValue
    {
        public int Int { get; init; }
        public string String { get; init; }
        public double Double { get; init; }
        public ElementId ElementId { get; init; }

        public bool IsArgumentInt { get; init; }
     


        public ArgumentValue(string s, int i, double d)
        {
            Int = i;
            String = s;
            Double = d;
            ElementId = ElementIdFactory.Create(i);
        }


        public static ArgumentValue Create(string text, ForgeTypeId dataTypeSpecId)
        {
            bool isInt = int.TryParse(text, out int intArg);
            bool isDouble = double.TryParse(text, out double doubleArg);

#if R2022_MIN
            if ((dataTypeSpecId != null) && (SpecUtils.IsValidDataType(dataTypeSpecId) && UnitUtils.IsMeasurableSpec(dataTypeSpecId)))
            {
                var units = Application.UIApplication?.ActiveUIDocument?.Document?.GetUnits();
                if (units != null)
                {
                    bool isRevitDouble = UnitFormatUtils.TryParse(units, dataTypeSpecId, text, out double revitDoubleArg);
                    if (isRevitDouble)
                    {
                        doubleArg = revitDoubleArg;
                    }
                }
            }
#endif

            return new ArgumentValue(text, intArg, doubleArg) { IsArgumentInt = isInt };
        }
    }
    internal class OperatorWithArgument 
    {        
        private readonly Operator @operator;
        private readonly string opArgument;

        public OperatorType Type => @operator.Type;


        public OperatorWithArgument(Operator @operator = null, string opArgument = "")
        {
            this.@operator = @operator ?? Operators.None;
            this.opArgument = opArgument.Trim();
        }


        public ArgumentValue Evaluate(ForgeTypeId dataTypeSpecId = null)
        {
            return ArgumentValue.Create(opArgument, dataTypeSpecId);
        }

        public string ToLabel(StorageType storageType, ForgeTypeId dataTypeSpecId)
        {
            if ((Type == OperatorType.HasValue) || (Type == OperatorType.HasNoValue) || (Type == OperatorType.Exists))
            {
                return @operator.Symbol;
            }

            var evaluated = Evaluate(dataTypeSpecId);

            string arg = $"\"{opArgument}\"";

            if (storageType == StorageType.Integer)
            {
                arg = evaluated.Int.ToString();
            }
            if (storageType == StorageType.ElementId && evaluated.IsArgumentInt)
            {
                arg = $"new ElementId({evaluated.Int})";
            }
            if (storageType == StorageType.Double)
            {                
                arg = evaluated.Double.ToString();
            }
                  
            return $"{@operator.Symbol} {arg}";
        }
    }
}