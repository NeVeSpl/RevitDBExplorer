using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class RuleBasedFilterCmdDefinition : ICommandDefinition, INeedInitializationWithDocument, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("f: ", "f:[filter]", "select elements that pass rule-based filter defined in Revit", AutocompleteItemGroups.Commands);
        private readonly DataBucket<RuleBasedFilterCmdArgument> dataBucket = new DataBucket<RuleBasedFilterCmdArgument>(0.61);


        public void Init(Document document)
        {
            dataBucket.Clear();
            foreach (var element in new FilteredElementCollector(document).OfClass(typeof(ParameterFilterElement)))
            {
                dataBucket.Add(new AutocompleteItem(element.Name, element.Name, null), new RuleBasedFilterCmdArgument(element.Id, element.Name), element.Name);
                
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
            yield return "f";
            yield return "filter";        
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
            return new RuleBasedFilterCmd(cmdText, args);
        }
    }


    internal class RuleBasedFilterCmdArgument : CommandArgument<ElementId>
    {
        public RuleBasedFilterCmdArgument(ElementId filterId, string name) : base(filterId)
        {           
            Name = $"new ElementId({filterId})";
            Label = name;
        }
    }


    internal class RuleBasedFilterCmd : Command, ICommandForVisualization
    {
        public string Label => "Rule-based filter: " + String.Join(", ", Arguments.Select(x => x.Label));
        public string Description => "A filter used to match elements which pass rule-based filter defined in Revit.";
        public string APIDescription => "ParameterFilterElement.GetElementFilter()";
        public CmdType Type => CmdType.DocumentSpecific;


        public RuleBasedFilterCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
        }
    }
}