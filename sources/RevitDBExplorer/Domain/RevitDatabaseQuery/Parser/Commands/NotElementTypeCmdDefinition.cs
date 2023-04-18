using System.Collections.Generic;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class NotElementTypeCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("element", "element", "select elements", AutocompleteItemGroups.Commands);


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
             

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
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            return new NotElementTypeCmd(cmdText);
        }
    }


    internal class NotElementTypeCmd : Command, ICommandForVisualization
    {
        public string Label => "element";
        public string Description => "A filter used to match elements which are not ElementTypes.";
        public string APIDescription => "collector.WhereElementIsNotElementType() or or new ElementIsElementTypeFilter(false)";
        public CmdType Type => CmdType.WithoutArgument;


        public NotElementTypeCmd(string text) : base(text, null, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}