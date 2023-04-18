using System.Collections.Generic;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class OwnerViewFilterCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("owned", "owned", "select elements which are owned by a active view", AutocompleteItemGroups.Commands);


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;


        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "owned";
            yield return "owned by view";
            yield return "owned by active view";
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            return new OwnerViewFilterCmd(cmdText);
        }
    }


    internal class OwnerViewFilterCmd : Command, ICommandForVisualization
    {
        public string Label => "owned by active view";
        public string Description => "A filter used to match elements which are owned by a particular view.";
        public string APIDescription => "new ElementOwnerViewFilter(document.ActiveView.Id)";
        public CmdType Type => CmdType.WithoutArgument;


        public OwnerViewFilterCmd(string text) : base(text, null, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}