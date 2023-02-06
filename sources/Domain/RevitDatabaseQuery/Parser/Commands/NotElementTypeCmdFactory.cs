using System;
using System.Collections.Generic;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class NotElementTypeCmdFactory : ICommandFactory
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("element ", "element", "select elements");

        public IAutocompleteItem GetAutocompleteItem() => AutocompleteItem;
      
        public Type MatchType => null;

        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "element";
            yield return "elements";
            yield return "not element type";
            yield return "not type";       
        }
        public bool CanRecognizeArgument(string argument)
        {
            return false;
        }

        public ICommand Create(string cmdText, IList<ILookupResult> arguments)
        {
            return new Command(CmdType.NotElementType, cmdText, null, null) { IsBasedOnQuickFilter = true };
        }
        public IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            yield break;
        }
    }
}