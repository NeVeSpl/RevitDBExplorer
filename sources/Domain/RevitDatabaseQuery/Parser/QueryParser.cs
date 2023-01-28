using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal class QueryParser
    {
        public static IList<string> Parse(string query)
        {
            IList<string> splitted = query.Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries).ToArray();
            splitted = ReconcilePotentialDoubleNumbers(splitted)
                                      .Where(x => !string.IsNullOrWhiteSpace(x))
                                      .Select(x => x.Trim())
                                      .ToArray();

            return splitted;
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
    }
}