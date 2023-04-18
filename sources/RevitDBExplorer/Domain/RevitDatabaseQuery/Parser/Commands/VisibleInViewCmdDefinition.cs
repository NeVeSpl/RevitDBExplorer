using System.Collections.Generic;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class VisibleInViewCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("visible", "visible", "select visible elements from the active view", AutocompleteItemGroups.Commands);


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;  
                    

        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {            
            yield return "visible";
            yield return "visible in view";       
            yield return "visible in active view";
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            return new VisibleInViewCmd(cmdText);
        }
    }


    internal class VisibleInViewCmd : Command, ICommandForVisualization
    {
        public string Label => "visible in active view";
        public string Description => "A quick filter that passes elements that are most likely visible in the given view.";
        public string APIDescription => "new VisibleInViewFilter(document, document.ActiveView.Id)";
        public CmdType Type => CmdType.WithoutArgument;


        public VisibleInViewCmd(string text) : base(text, null, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}