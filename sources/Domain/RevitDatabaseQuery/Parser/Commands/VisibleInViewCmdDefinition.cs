using System.Collections.Generic;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class VisibleInViewCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("active view", "active view", "select elements from the active view");


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;  
                    

        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "active";
            yield return "active view";
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            return new VisibleInViewCmd(cmdText);
        }
    }


    internal class VisibleInViewCmd : Command
    {
        public VisibleInViewCmd(string text) : base(CmdType.ActiveView, text, null, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}