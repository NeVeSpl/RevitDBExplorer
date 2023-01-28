using System;
using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal interface ICommandFactory
    {
        Type MatchType { get; }

        IEnumerable<string> GetClassifiers();
        IEnumerable<string> GetKeywords();
        bool CanRecognizeArgument(string argument);

        ICommand Create(string cmdText, IList<ILookupResult> arguments);
        IEnumerable<ILookupResult> ParseArgument(string argument);
    }

    internal abstract class CommandFactory<T> : ICommandFactory  where T : ILookupResult
    {
        public Type MatchType => typeof(T);

        public abstract bool CanRecognizeArgument(string argument);
        public abstract ICommand Create(string cmdText, IList<ILookupResult> arguments);
        public abstract IEnumerable<string> GetClassifiers();
        public abstract IEnumerable<string> GetKeywords();
        public abstract IEnumerable<ILookupResult> ParseArgument(string argument);
    }
}