using System.Collections.Generic;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal interface ICommand
    {
        CmdType Type { get;  }
        string Text { get; }
        IEnumerable<ILookupResult> MatchedArguments { get;  }
        OperatorWithArgument Operator { get; }
    }
}