using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class ElementIdCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("i: ", "i:[number] ", "select elements with given id", AutocompleteItemGroups.Commands);
        private readonly DataBucket<ElementIdCmdArgument> dataBucket = new DataBucket<ElementIdCmdArgument>(0.666);


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
      

        public IEnumerable<string> GetClassifiers()
        {
            yield return "i";
            yield return "id";
            yield return "ids";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            if (long.TryParse(argument, out long longValue))
            {
                return true;
            }
            return false;
        }
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            long.TryParse(argument, out long longValue);
            var id = ElementIdFactory.Create(longValue);
            
            return new ElementIdCmd(cmdText, dataBucket.CreateMatch(new ElementIdCmdArgument(id)));
        }
    }


    internal class ElementIdCmdArgument : CommandArgument<ElementId>
    {
        public ElementIdCmdArgument(ElementId value) : base(value)
        {           
            Name = $"new ElementId({value})";
            Label = value.Value().ToString();
        }
    }


    internal class ElementIdCmd : Command, ICommandForVisualization
    {
        public string Label => String.Join(", ", Arguments.Select(x => x.Name));
        public string Description => "A filter used to match elements by their id.";
        public string APIDescription => "new ElementIdSetFilter()";
        public CmdType Type => CmdType.ElementId;


        public ElementIdCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}