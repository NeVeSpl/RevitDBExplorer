using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal interface ICommand
    {
        CmdType Type { get;  }
        string Text { get; }
        IEnumerable<ICommandArgument> MatchedArguments { get;  }
        OperatorWithArgument Operator { get; }
        bool IsBasedOnQuickFilter { get; init; }

        double Score { get; }
    }


    internal class Command : ICommand
    {
        public CmdType Type { get; init; } = CmdType.Incorrect;
        public string Text { get; init; } = "";
        public IEnumerable<ICommandArgument> MatchedArguments { get; init; } = Enumerable.Empty<ICommandArgument>();
        public OperatorWithArgument Operator { get; init; } = new OperatorWithArgument();
        public bool IsBasedOnQuickFilter { get; init; } = false;


        public double Score 
        {
            get
            {
                if (MatchedArguments.Any())
                {
                    return (MatchedArguments.First() as IFuzzySearchResult).LevensteinScore;
                }
                return 0;
            }
        }


        public Command(CmdType type, string text, IEnumerable<ICommandArgument> matchedArguments = null, OperatorWithArgument @operator = null)
        {
            Type = type;
            Text = text;
            MatchedArguments = matchedArguments ?? MatchedArguments;
            Operator = @operator ?? Operator;
        }
    }
}