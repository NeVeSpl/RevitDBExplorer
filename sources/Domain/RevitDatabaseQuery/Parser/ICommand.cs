using System.Collections.Generic;
using System.Linq;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal interface ICommand
    {
        CmdType Type { get;  }
        string Text { get; }
        IEnumerable<ILookupResult> MatchedArguments { get;  }
        OperatorWithArgument Operator { get; }
        bool IsBasedOnQuickFilter { get; init; }
    }


    internal class Command : ICommand
    {
        public CmdType Type { get; init; } = CmdType.Incorrect;
        public string Text { get; init; } = "";
        public IEnumerable<ILookupResult> MatchedArguments { get; init; } = Enumerable.Empty<ILookupResult>();
        public OperatorWithArgument Operator { get; init; } = new OperatorWithArgument();
        public bool IsBasedOnQuickFilter { get; init; } = false;


        public Command(CmdType type, string text, IEnumerable<ILookupResult> matchedArguments = null, OperatorWithArgument @operator = null)
        {
            Type = type;
            Text = text;
            MatchedArguments = matchedArguments ?? MatchedArguments;
            Operator = @operator ?? Operator;
        }
    }
}