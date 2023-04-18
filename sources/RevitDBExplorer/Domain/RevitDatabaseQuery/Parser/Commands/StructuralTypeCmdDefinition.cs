using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class StructuralTypeCmdDefinition : ICommandDefinition, INeedInitialization, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem CmdDefAutocompleteItem = new AutocompleteItem("s: ", "s:[structural type]", "select elements matching a structural type", AutocompleteItemGroups.Commands);
        private readonly DataBucket<StructuralTypeCmdArgument> dataBucket = new DataBucket<StructuralTypeCmdArgument>(0.61);


        public void Init()
        {
            var values = System.Enum.GetValues(typeof(StructuralType));
         
            foreach (StructuralType i in values)
            {
                var name = Enum.GetName(typeof(StructuralType), i);
                var label = $"StructuralType.{name}";
                dataBucket.Add(new AutocompleteItem(label, label, name), new StructuralTypeCmdArgument(i), name);
            }

            dataBucket.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem() => CmdDefAutocompleteItem;
        public IEnumerable<IAutocompleteItem> GetAutocompleteItems(string prefix)
        {            
            return dataBucket.ProvideAutoCompletion(prefix);            
        }


        public IEnumerable<string> GetClassifiers()
        {
            yield return "s";
            yield return "stru";
            yield return "structual";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            if (argument.StartsWith(nameof(StructuralType), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        public bool CanParticipateInGenericSearch() => false;
              

        public ICommand Create(string cmdText, string argument)
        {
            var arg = argument.RemovePrefix("StructuralType.");
            var args = dataBucket.FuzzySearch(arg);
            return new StructuralTypeCmd(cmdText, args);
        }        
    }


    internal class StructuralTypeCmdArgument : CommandArgument<StructuralType>
    {
        public StructuralTypeCmdArgument(StructuralType value) : base(value)
        {           
            Name = $"StructuralType.{value}";
        }
    }


    internal class StructuralTypeCmd : Command, ICommandForVisualization
    {
        public string Label => String.Join(", ", Arguments.Select(x => x.Name));
        public string Description => "A filter used to find elements matching a structural type.";
        public string APIDescription => "new ElementStructuralTypeFilter()";
        public CmdType Type => CmdType.EnumBased;


        public IEnumerable<StructuralTypeCmdArgument> Arguments { get; }


        public StructuralTypeCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments) : base(text, matchedArguments, null)
        {
            Arguments = matchedArguments.Select(x => x.Argument as StructuralTypeCmdArgument)?.ToArray();
            IsBasedOnQuickFilter = true;
        }
    }   
}