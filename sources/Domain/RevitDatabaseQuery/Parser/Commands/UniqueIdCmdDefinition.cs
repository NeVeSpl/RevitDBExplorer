using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class UniqueIdCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("u: ", "u:[guid] ", "select elements with given UniqueId", AutocompleteItemGroups.Commands);
        private readonly DataBucket<UniqueIdCmdArgument> dataBucket = new DataBucket<UniqueIdCmdArgument>(0.666);


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;


        public IEnumerable<string> GetClassifiers()
        {
            yield return "u";
            yield return "uid";
            yield return "unique";
            yield return "unique id";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            if (argument.Length >= 36)
            { 
                var guidPart = argument.Substring(0, 36);

                if (Guid.TryParse(guidPart, out Guid guidValue))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            return new UniqueIdCmd(cmdText, dataBucket.CreateMatch(new UniqueIdCmdArgument(argument.Trim())));
        }
    }


    internal class UniqueIdCmdArgument : CommandArgument<string>
    {
        public UniqueIdCmdArgument(string value) : base(value)
        {
            Name = $"\"{value}\"";
            Label = value;
        }
    }


    internal class UniqueIdCmd : Command, ICommandForVisualization
    {
        public string Label => String.Join(", ", Arguments.Select(x => x.Name));
        public string Description => "A filter used to match elements by their UniqueId.";
        public string APIDescription => "document.GetElement()";
        public CmdType Type => CmdType.ElementId;


        public UniqueIdCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
            IsBasedOnQuickFilter = false;
        }
    }
}