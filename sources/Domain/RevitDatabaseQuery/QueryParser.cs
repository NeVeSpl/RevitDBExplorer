using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery
{
    internal class QueryParser
    {
        public static List<ICommand> Parse(string query)
        {            
            var commandStrings = SplitIntoCmdStrings(query).Select(x => x.Trim());
            var commands = commandStrings.SelectMany(x => CommandParser.Parse(x)).ToList();          

            if (!DoesContainQuickFilter(commands))
            {
                commands.Insert(0, CommandParser.Parse("type").First());
                commands.Insert(0, CommandParser.Parse("element").First());
            }

            return commands.OfType<ICommand>().ToList();
        }

        public static readonly char[] Separators = new[] { ';', ',' };
        public static IList<string> SplitIntoCmdStrings(string query)
        {            
            IList<string> splitted = query.Split(Separators, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var commandsText = ReconcilePotentialDoubleNumbers(splitted)
                                      .Where(x => !string.IsNullOrWhiteSpace(x))
                                      
                                      .ToArray();
            return commandsText;
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

       
        private static bool DoesContainQuickFilter(IList<ICommand> commands)
        {
            foreach (var command in commands)
            {
                if (command.IsBasedOnQuickFilter)
                {
                    return true;
                }                
            }
            return false;
        }
    }
}