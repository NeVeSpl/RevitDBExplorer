using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class LevelCmdDefinition : ICommandDefinition, INeedInitializationWithDocument, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("l: ", "l:[level]", "select elements from a given level", AutocompleteItemGroups.Commands);
        private readonly DataBucket<LevelCmdArgument> dataBucket = new DataBucket<LevelCmdArgument>(0.61);

      
        public void Init(Document document)
        {
            dataBucket.Clear();
            foreach (var element in new FilteredElementCollector(document).OfClass(typeof(Level)))
            {
                dataBucket.Add(new AutocompleteItem(element.Name, element.Name, null), new LevelCmdArgument(element.Id, element.Name), element.Name);
            }
            dataBucket.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
        public IEnumerable<IAutocompleteItem> GetAutocompleteItems(string prefix)
        {
            return dataBucket.ProvideAutoCompletion(prefix);
        }


        public IEnumerable<string> GetClassifiers()
        {
            yield return "l";
            yield return "lvl";
            yield return "level";         
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => true;


        public ICommand Create(string cmdText, string argument)
        {
            var args = dataBucket.FuzzySearch(argument);
            return new LevelCmd(cmdText, args);
        }
    }


    internal class LevelCmdArgument : CommandArgument<ElementId>
    {
        public LevelCmdArgument(ElementId levelId, string name) : base(levelId)
        {           
            Name = $"new ElementId({levelId})";
            Label = name;
        }
    }


    internal class LevelCmd : Command, ICommandForVisualization
    {
        public string Label => "Level: " + String.Join(", ", Arguments.Select(x => x.Label));
        public string Description => "A filter used to match elements by their associated level. A slow filter.";
        public string APIDescription => "new ElementLevelFilter()";
        public CmdType Type => CmdType.DocumentSpecific;


        public LevelCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
        }
    }
}