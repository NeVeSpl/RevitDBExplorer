using System;
using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class ElementTypeCmdFactory : ICommandFactory
    {
        public Type MatchType => null;

        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "element type";
            yield return "not element";
            yield return "type";
            yield return "types";
        }
        public bool CanRecognizeArgument(string argument)
        {
            return false;
        }

        public ICommand Create(string cmdText, IList<ILookupResult> arguments)
        {
            return new Command(CmdType.ElementType, cmdText, null, null) { IsBasedOnQuickFilter = true };
        }
        public IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            yield break;
        }
    }
}