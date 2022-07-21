using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal enum CmdType
    {
        ActiveView,
        View,
        ElementId,
        ElementType,
        NotElementType,
        Category,
        Class,
        NameParam,
        Parameter,
        Incorrect = 383,
        WhoKnows = 666
    }
    internal enum Operator { None, Equals, Greater, GreaterOrEqual, Less, LessOrEqual, HasNoValue, HasValue, NotEquals }

    internal class QueryParser
    {
        public static List<Command> Parse(string query)
        {
            var splitted = query.Trim().Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            var commands = splitted.Select(f => new Command(f.Trim())).ToList();

            if (!DoesContainQuickFilter(commands))
            {
                commands.Insert(0, new Command("type"));
                commands.Insert(0, new Command("element"));
            }

            return commands;
        }

        private static readonly HashSet<CmdType> quickFilters = new() { CmdType.ActiveView, CmdType.ElementId, CmdType.ElementType, CmdType.NotElementType, CmdType.Category, CmdType.Class };

        private static bool DoesContainQuickFilter(List<Command> commands)
        {
            foreach (var command in commands)
            {
                if (quickFilters.Contains(command.Type))
                {
                    return true;
                }
                if (command.Type == CmdType.WhoKnows)
                {
                    foreach (var arg in command.Arguments)
                    {
                        if (arg.IsClass || arg.IsCategory || arg.IsElementId)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    internal class Command
    {
        public CmdType Type { get; init; } = CmdType.WhoKnows;
        public string Argument { get; init; } = "";
        public IEnumerable<ILookupResult> Arguments { get; init; } = Enumerable.Empty<ILookupResult>();
        public Operator Operator { get; init; } = Operator.None;
        public string OperatorArgumentAsString { get; init; } = "";
        public double OperatorArgumentAsDouble { get; init; } = double.NaN;
        public int OperatorArgumentAsInt { get; init; } = 0;


        public Command(string cmd)
        {
            var splitted = cmd.Split(new[] { ':' }, 2, System.StringSplitOptions.RemoveEmptyEntries);

            if (splitted.Length == 1)
            {
                Type = InterpretCommandType(splitted[0]);
                Argument = splitted[0].Trim();
            }
            if (splitted.Length == 2)
            {
                Type = ParseCommandClassifier(splitted[0]);
                Argument = splitted[1].Trim();                
            }

            if (Type == CmdType.WhoKnows)
            {
                if (Argument.StartsWith(nameof(BuiltInCategory), StringComparison.OrdinalIgnoreCase))
                {
                    Type = CmdType.Category;
                }
                if (Argument.StartsWith(nameof(BuiltInParameter), StringComparison.OrdinalIgnoreCase) || Argument.IndexOfAny(operators) > 0)
                {
                    Type = CmdType.Parameter;
                }
            }

            Arguments = ParseArgument(Type, Argument);

            if (Arguments.IsEmpty())
            {
                if (Type == CmdType.ElementId || Type == CmdType.Category || Type == CmdType.Class || Type == CmdType.Parameter)
                {
                    Type = CmdType.Incorrect;
                    Argument = cmd;
                }
                if (Type == CmdType.WhoKnows)
                {
                    Type = CmdType.NameParam;
                }
            }
            else
            {
                if (Type == CmdType.WhoKnows)
                {
                    if (Arguments.All(x => x.IsClass)) Type = CmdType.Class;
                    if (Arguments.All(x => x.IsCategory)) Type = CmdType.Category;
                    if (Arguments.All(x => x.IsElementId)) Type = CmdType.ElementId;
                }
            }

            if (Type == CmdType.Parameter)
            {
                if (Argument.Contains("=")) Operator = Operator.Equals;
                if (Argument.Contains("<")) Operator = Operator.Less;
                if (Argument.Contains("<=")) Operator = Operator.LessOrEqual;
                if (Argument.Contains(">")) Operator = Operator.Greater;
                if (Argument.Contains(">=")) Operator = Operator.GreaterOrEqual;
                if (Argument.Contains("??")) Operator = Operator.HasNoValue;
                if (Argument.Contains("!!")) Operator = Operator.HasValue;
                if (Argument.Contains("!=")) Operator = Operator.NotEquals;
                if (Argument.Contains("<>")) Operator = Operator.NotEquals;

                var splittedByOpp = Argument.Split(operators, 2, System.StringSplitOptions.RemoveEmptyEntries);
                if (splittedByOpp.Length > 1)
                {
                    OperatorArgumentAsString = splittedByOpp.Last().Trim();
                }           
            }
           
        }

        
        char[] operators = new char[] { '<', '=', '>', '?', '!' };

        private CmdType InterpretCommandType(string strType)
        {
            var needle = strType.ToLower().RemoveWhitespace();
            switch (needle)
            {
                case "active":
                case "activeview":
                    return CmdType.ActiveView;
                case "elementtype":
                case "notelement":
                case "type":
                case "types":
                    return CmdType.ElementType;
                case "element":
                case "notelementtype":
                case "elements":
                    return CmdType.NotElementType;
            }

            return CmdType.WhoKnows;
        }
        private CmdType ParseCommandClassifier(string strType)
        {
            var needle = strType.ToLower().RemoveWhitespace();
            switch (needle)
            {
                //case "view":
                //   return CmdType.View;
                case "id":
                case "ids":
                    return CmdType.ElementId;
                case "category":
                case "cat":
                    return CmdType.Category;
                case "type":
                case "class":
                case "typeof":
                    return CmdType.Class;
                case "name":
                    return CmdType.NameParam;
                //case "par":
                //case "param":
                //case "parameter":
                //    return CmdType.Parameter;
            }
            return CmdType.WhoKnows;
        }
        private IEnumerable<ILookupResult> ParseArgument(CmdType cmdTpe, string argument)
        {
            IEnumerable<ILookupResult> result = null;
            switch (cmdTpe)
            {
                case CmdType.ActiveView:
                case CmdType.ElementType:
                case CmdType.NotElementType:
                    // do not have arguments
                    break;
                case CmdType.View:
                    break;
                case CmdType.ElementId:
                    result = FuzzySearchEngine.Lookup(argument, LookupFor.ElementId).ToList();
                    break; 
                case CmdType.Category:
                    result = FuzzySearchEngine.Lookup(argument, LookupFor.Category).ToList();
                    break;
                case CmdType.Class:
                    result = FuzzySearchEngine.Lookup(argument, LookupFor.Class).ToList();
                    break;
                case CmdType.NameParam:
                    break;
                case CmdType.Parameter:
                    string paramName = argument.Split(operators, 2, System.StringSplitOptions.RemoveEmptyEntries)[0];
                    result = FuzzySearchEngine.Lookup(paramName, LookupFor.Parameter).ToList();
                    break;
                case CmdType.WhoKnows:
                    result = FuzzySearchEngine.Lookup(argument, LookupFor.ElementId | LookupFor.Category | LookupFor.Class).ToList();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result ?? Enumerable.Empty<ILookupResult>();
        }
    }
}