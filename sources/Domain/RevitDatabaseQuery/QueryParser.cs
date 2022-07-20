using System;
using System.Collections.Generic;
using System.Linq;

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

    internal class QueryParser
    {
        public static List<Command> Parse(string query)
        {
            var splitted = query.Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            var commands = splitted.Select(f => new Command(f)).ToList();

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

            Arguments = ParseArgument(Type, Argument);

            if (Arguments.IsEmpty())
            {
                if (Type == CmdType.ElementId || Type == CmdType.Category || Type == CmdType.Class)
                {
                    Type = CmdType.Incorrect;
                }
                if (Type == CmdType.WhoKnows)
                {
                    Type = CmdType.NameParam;
                }
            }
        }
       

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
                case "class":
                    return CmdType.Class;
                case "name":
                    return CmdType.NameParam;
                case "param":
                case "parameter":
                    return CmdType.Parameter;
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
                    break;
                case CmdType.WhoKnows:
                    result = FuzzySearchEngine.Lookup(argument, LookupFor.All).ToList();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result ?? Enumerable.Empty<ILookupResult>();
        }
    }
}