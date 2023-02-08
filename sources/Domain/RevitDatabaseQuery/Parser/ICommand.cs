using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal interface ICommand
    {
        CmdType Type { get;  }
        string Text { get; }
        IEnumerable<IFuzzySearchResult> MatchedArguments { get;  }
        OperatorWithArgument Operator { get; }
        bool IsBasedOnQuickFilter { get; init; }

        IEnumerable<ICommandArgument> Arguments { get; }

        double Score { get; }
    }


    internal class Command : ICommand
    {
        public CmdType Type { get; init; } = CmdType.Incorrect;
        public string Text { get; init; } = "";
        public IEnumerable<IFuzzySearchResult> MatchedArguments { get; init; } = Enumerable.Empty<IFuzzySearchResult>();
        public OperatorWithArgument Operator { get; init; } = new OperatorWithArgument();
        public bool IsBasedOnQuickFilter { get; init; } = false;


        public double Score 
        {
            get
            {
                if (MatchedArguments.Any())
                {
                    return MatchedArguments.First().LevensteinScore;
                }
                return 0;
            }
        }

        public IEnumerable<ICommandArgument> Arguments 
        {
            get => MatchedArguments.Select(x => x.Argument);
            
        }


        public Command(CmdType type, string text, IEnumerable<IFuzzySearchResult> matchedArguments = null, OperatorWithArgument @operator = null)
        {
            Type = type;
            Text = text;
            MatchedArguments = matchedArguments?.ToArray() ?? MatchedArguments;
            Operator = @operator ?? Operator;
        }
    }
}