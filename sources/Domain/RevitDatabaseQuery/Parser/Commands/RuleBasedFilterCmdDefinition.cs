using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class RuleBasedFilterCmdDefinition : ICommandDefinition, INeedInitializationWithDocument
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("f: ", "f:[filter]", "select elements that pass rule-based filter defined in Revit");
        private readonly DataBucket<RuleBasedFilterCmdArgument> dataBucket = new DataBucket<RuleBasedFilterCmdArgument>(0.61);


        public void Init(Document document)
        {
            dataBucket.Clear();
            foreach (var element in new FilteredElementCollector(document).OfClass(typeof(ParameterFilterElement)))
            {
                dataBucket.Add(null, new RuleBasedFilterCmdArgument(element.Id, element.Name), element.Name);
                
            }
            dataBucket.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
       

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
            CmdType = CmdType.RuleBasedFilter;
            Name = $"new ElementId({filterId})";
            Label = name;
        }
    }


    internal class RuleBasedFilterCmd : Command
    {
        public RuleBasedFilterCmd(string text, IEnumerable<ICommandArgument> matchedArguments = null) : base(CmdType.RuleBasedFilter, text, matchedArguments, null)
        {
        }
    }
}