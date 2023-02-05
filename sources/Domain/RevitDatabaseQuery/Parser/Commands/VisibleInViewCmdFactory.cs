using System;
using System.Collections.Generic;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class VisibleInViewCmdFactory : ICommandFactory
    {
        public IAutocompleteItem GetAutocompleteItem() => new AutocompleteItem("active - select elements from the active view", "active");

        public Type MatchType => null;

        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "active";
            yield return "active view";
        }
        public bool CanRecognizeArgument(string argument)
        {           
            return false;
        }


        public ICommand Create(string cmdText, IList<ILookupResult> arguments)
        {
            return new Command(CmdType.ActiveView, cmdText, arguments, null) { IsBasedOnQuickFilter = true };
        }
        public IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            yield break;
        }
    }
}