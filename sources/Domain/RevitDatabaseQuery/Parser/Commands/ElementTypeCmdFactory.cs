using System;
using System.Collections.Generic;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class ElementTypeCmdFactory : ICommandFactory
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("type ", "type", "select element types");

        public IAutocompleteItem GetAutocompleteItem() => AutocompleteItem;
              

        public Type MatchType => null;

        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "type";
            yield return "types";
            yield return "element type";
            yield return "not element";           
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