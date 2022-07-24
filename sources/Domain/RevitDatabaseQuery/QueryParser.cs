using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
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
        NameParam,
        Parameter,
        Incorrect = 383,
        WhoKnows = 666
    }
    

    internal class QueryParser
    {
        public static List<Command> Parse(string query)
        {
            var splitted = query.Trim().Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            var commands = splitted.Select(c => new Command(c)).ToList();

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
                    foreach (var arg in command.MatchedArguments)
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
        public string Text { get; init; } = "";
        private string Argument { get; init; } = "";
        public IEnumerable<ILookupResult> MatchedArguments { get; init; } = Enumerable.Empty<ILookupResult>();
        public OperatorWithArgument Operator { get; init; } = new OperatorWithArgument();        


        public Command(string cmdText)
        {
            Text = cmdText.Trim();
            var splittedByClassifier = Text.Split(new[] { ':' }, 2, System.StringSplitOptions.RemoveEmptyEntries);

            if (splittedByClassifier.Length == 1)
            {
                Type = InterpretCommandType(splittedByClassifier[0]);
                Argument = splittedByClassifier[0].Trim();
            }
            if (splittedByClassifier.Length == 2)
            {
                Type = ParseCommandClassifier(splittedByClassifier[0]);
                Argument = splittedByClassifier[1].Trim();                
            }

            if (Type == CmdType.WhoKnows)
            {
                if (Argument.StartsWith(nameof(BuiltInCategory), StringComparison.OrdinalIgnoreCase))
                {
                    Argument = Argument.Remove(0, nameof(BuiltInCategory).Length + 1);
                    Type = CmdType.Category;
                }
                if (Argument.StartsWith(nameof(BuiltInParameter), StringComparison.OrdinalIgnoreCase))
                {
                    Argument = Argument.Remove(0, nameof(BuiltInParameter).Length + 1);
                    Type = CmdType.Parameter;
                }
                if (Operators.DoesContainAnyValidOperator(Argument))
                {
                    Type = CmdType.Parameter;
                }
            }

            if (Type == CmdType.Parameter)
            {
                Operator = Operators.Parse(Argument);
                if (Operator.Type != OperatorType.None)
                {
                    Argument = Argument.Substring(0, Argument.IndexOf(Operator.Symbol));
                }
                else
                {
                    Type = CmdType.Incorrect;
                }
            }

            MatchedArguments = ParseArgument(Type, Argument);

            if (MatchedArguments.IsEmpty())
            {
                if (Type == CmdType.ElementId || Type == CmdType.Category || Type == CmdType.Class || Type == CmdType.Parameter)
                {
                    Type = CmdType.Incorrect;                   
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
                    if (MatchedArguments.All(x => x.IsClass)) Type = CmdType.Class;
                    if (MatchedArguments.All(x => x.IsCategory)) Type = CmdType.Category;
                    if (MatchedArguments.All(x => x.IsElementId)) Type = CmdType.ElementId;
                }
            }     
            
            if (Type == CmdType.NameParam)
            {
                if (string.IsNullOrEmpty(Argument))
                {
                    Type = CmdType.Incorrect;
                }
                else
                {
                    Type = CmdType.Parameter;
                    MatchedArguments = NameLikeParameters.Select(x => new BuiltInParameterMatch(x, 1)).ToList();
                    Operator = Operators.Parse($"=%{Argument}%");
                }
            }
        }
        private static readonly List<BuiltInParameter> NameLikeParameters = new List<BuiltInParameter>()
        {
            BuiltInParameter.ALL_MODEL_TYPE_NAME,
            BuiltInParameter.ALL_MODEL_MARK,
            BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM,
            BuiltInParameter.DATUM_TEXT
        };

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
                case "elements":
                case "notelementtype":
                case "nottype":               
                    return CmdType.NotElementType;
            }

            return CmdType.WhoKnows;
        }
        private CmdType ParseCommandClassifier(string strType)
        {
            var needle = strType.ToLower().RemoveWhitespace();
            switch (needle)
            {                
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
                    result = FuzzySearchEngine.Lookup(argument, LookupFor.Parameter).ToList();
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