using System.Collections.Generic;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class SelectionCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("selection", "selection", "retrieve the currently selected Elements in Revit", AutocompleteItemGroups.Commands);


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;


        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "selection";           
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            return new SelectionCmd(cmdText);
        }
    }


    internal class SelectionCmd : Command, ICommandForVisualization
    {
        public string Label => "selection in Revit";
        public string Description => "Retrieve the currently selected Elements in Revit.";
        public string APIDescription => "ActiveUIDocument.Selection.GetElementIds()";
        public CmdType Type => CmdType.WithoutArgument;


        public SelectionCmd(string text) : base(text, null, null)
        {
            IsBasedOnQuickFilter = false;
        }
    }
}
