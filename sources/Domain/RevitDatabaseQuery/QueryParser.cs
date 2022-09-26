using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal enum CmdType
    {
        ActiveView,       
        ElementId,
        ElementType,
        NotElementType,
        Category,
        Class,
        NameParam, // only inside CommandFactory
        Parameter,
        StructuralType,
        Level,
        Room,
        RuleBasedFilter,
        Incorrect = 383,
        WhoKnows = 666 // only inside CommandFactory
    }
    

    internal class QueryParser
    {
        public static List<Command> Parse(string query)
        {
            IList<string> splitted = query.Trim().Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            splitted = ReconcilePotentialDoubleNumbers(splitted).ToList();
            var commands = splitted.SelectMany(c => CommandFactory.Create(c)).ToList();

            if (!DoesContainQuickFilter(commands))
            {
                commands.Insert(0, CommandFactory.Create("type").First());
                commands.Insert(0, CommandFactory.Create("element").First());
            }

            return commands;
        }

        private static IEnumerable<string> ReconcilePotentialDoubleNumbers(IList<string> splitted)
        {
            int i;
            for (i = 0; i < splitted.Count - 1; i++)
            {
                var isParam = Operators.DoesContainAnyValidOperator(splitted[i]);
                var isNumber = char.IsNumber(splitted[i + 1][0]);
                if (isParam && isNumber)
                {
                    yield return String.Concat(splitted[i], ",", splitted[i + 1]);
                    ++i;
                } 
                else
                {
                    yield return splitted[i];                    
                }
            }
            if (i == splitted.Count - 1)
            {
                yield return splitted[i];
            }
        }

        private static readonly HashSet<CmdType> quickFilters = new() { CmdType.ActiveView, CmdType.ElementId, CmdType.ElementType, CmdType.NotElementType, CmdType.Category, CmdType.Class, CmdType.StructuralType };
        private static bool DoesContainQuickFilter(List<Command> commands)
        {
            foreach (var command in commands)
            {
                if (quickFilters.Contains(command.Type))
                {
                    return true;
                }                
            }
            return false;
        }
    }


    internal class Command
    {      
        public CmdType Type { get; init; } = CmdType.WhoKnows;
        public string Text { get; init; } = "";       
        public IEnumerable<ILookupResult> MatchedArguments { get; init; } = Enumerable.Empty<ILookupResult>();
        public OperatorWithArgument Operator { get; init; } = new OperatorWithArgument();


        public Command(CmdType type, string text, IEnumerable<ILookupResult> matchedArguments = null, OperatorWithArgument @operator = null)
        {
            Type = type;
            Text = text;
            MatchedArguments = matchedArguments ?? MatchedArguments;
            Operator = @operator ?? Operator;
        }
    }
    internal static class CommandFactory
    { 
        public static IEnumerable<Command> Create(string phrase)
        {
            var cmdText = phrase.Trim();
            var argument = "";
            var type = CmdType.WhoKnows;                    
            var splittedByClassifier = cmdText.Split(new[] { ':' }, 2, System.StringSplitOptions.RemoveEmptyEntries);

            if (splittedByClassifier.Length == 1)
            {
                type = InterpretCommandType(splittedByClassifier[0]);
                argument = splittedByClassifier[0].Trim();
            }
            if (splittedByClassifier.Length == 2)
            {
                type = ParseCommandClassifier(splittedByClassifier[0]);
                argument = splittedByClassifier[1].Trim();                
            }

            // Try infer command type from argument 
            if (type == CmdType.WhoKnows)
            {
                if (argument.StartsWith(nameof(BuiltInCategory), StringComparison.OrdinalIgnoreCase))
                {
                    argument = argument.Remove(0, nameof(BuiltInCategory).Length + 1);
                    type = CmdType.Category;
                }
                if (argument.StartsWith(nameof(BuiltInParameter), StringComparison.OrdinalIgnoreCase))
                {
                    argument = argument.Remove(0, nameof(BuiltInParameter).Length + 1);
                    type = CmdType.Parameter;
                }
                if (argument.StartsWith(nameof(StructuralType), StringComparison.OrdinalIgnoreCase))
                {
                    argument = argument.Remove(0, nameof(StructuralType).Length + 1);
                    type = CmdType.StructuralType;
                }
                if (Operators.DoesContainAnyValidOperator(argument))
                {
                    type = CmdType.Parameter;
                }
            }

            // Parse operator
            var @operator = new OperatorWithArgument();
            if (type == CmdType.Parameter)
            {
                @operator = Operators.Parse(argument);
                if (@operator.Type != OperatorType.None)
                {
                    argument = argument.Substring(0, argument.IndexOf(@operator.Symbol));
                }
                else
                {
                    type = CmdType.Incorrect;
                }
            }

            var matchedArguments = ParseArgument(type, argument);
            if (matchedArguments != null)
            {
                if (matchedArguments.IsEmpty())
                {
                    if (type != CmdType.WhoKnows)
                    {
                        type = CmdType.Incorrect;
                    }
                    else                   
                    {
                        type = CmdType.NameParam;
                    }
                }
                else
                {
                    var groupedByType = matchedArguments.GroupBy(x => x.CmdType);
                    foreach (var group in groupedByType)
                    {
                        yield return new Command(group.Key, cmdText, group.ToList(), @operator);
                    }
                    yield break;
                }
            }
            
            if (type == CmdType.NameParam)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    type = CmdType.Incorrect;
                }
                else
                {
                    type = CmdType.Parameter;
                    matchedArguments = NameLikeParameters.Select(x => new ParameterMatch(x, 1)).ToList();
                    @operator = Operators.Parse($"=%{argument}%");
                }
            }

            yield return new Command(type, cmdText, matchedArguments, @operator);
        }
        private static readonly List<BuiltInParameter> NameLikeParameters = new List<BuiltInParameter>()
        {
            BuiltInParameter.ALL_MODEL_TYPE_NAME,
            BuiltInParameter.ALL_MODEL_MARK,
            BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM,
            BuiltInParameter.DATUM_TEXT
        };

        private static CmdType InterpretCommandType(string strType)
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
                case "elements":
                case "notelementtype":
                case "nottype":               
                    return CmdType.NotElementType;
            }

            return CmdType.WhoKnows;
        }
        private static CmdType ParseCommandClassifier(string strType)
        {
            var needle = strType.ToLower().RemoveWhitespace();
            switch (needle)
            {
                case "i":
                case "id":
                case "ids":
                    return CmdType.ElementId;
                case "c":
                case "cat":
                case "category":                
                    return CmdType.Category;
                case "t":
                case "type":
                case "class":
                case "typeof":
                    return CmdType.Class;
                case "n":
                case "name":
                    return CmdType.NameParam;
                case "s":
                case "stru":
                case "structual":
                    return CmdType.StructuralType;
                case "l":
                case "lvl":
                case "level":
                    return CmdType.Level;
                case "r":
                case "room":             
                    return CmdType.Room;
                case "f":
                case "filter":
                    return CmdType.RuleBasedFilter;
            }
            return CmdType.WhoKnows;
        }
        private static IEnumerable<ILookupResult> ParseArgument(CmdType cmdTpe, string argument)
        {
            IEnumerable<ILookupResult> result = null;
            switch (cmdTpe)
            {                            
                case CmdType.ElementId:
                    result = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.ElementId).ToList();
                    break; 
                case CmdType.Category:
                    result = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.Category).ToList();
                    break;
                case CmdType.Class:
                    result = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.Class).ToList();
                    break;
                case CmdType.StructuralType:
                    result = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.StructuralType).ToList();
                    break;
                case CmdType.Parameter:                    
                    result = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.Parameter).ToList();
                    break;
                case CmdType.Level:
                    result = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.Level).ToList();
                    break;
                case CmdType.Room:
                    result = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.Room).ToList();
                    break;
                case CmdType.RuleBasedFilter:
                    result = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.RuleBasedFilter).ToList();
                    break;
                case CmdType.WhoKnows:
                    result = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.ElementId | 
                                                                FuzzySearchEngine.LookFor.Category | 
                                                                FuzzySearchEngine.LookFor.Class | 
                                                                FuzzySearchEngine.LookFor.Level |
                                                                FuzzySearchEngine.LookFor.Room |
                                                                FuzzySearchEngine.LookFor.RuleBasedFilter 
                                                                ).ToList();
                    break;               
            }
            return result;
        }
    }
}