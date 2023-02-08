using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class NameCmdDefinition : ICommandDefinition
    {
        public static readonly NameCmdDefinition Instance = new NameCmdDefinition();
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("n: ", "n:[text]", "wildcard search for a given text");


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
            

        public IEnumerable<string> GetClassifiers()
        {
            yield return "n";
            yield return "name";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            var matchedArguments = NameLikeParameters.Select(x => new ParameterMatch(x)).ToArray();

            var arg = cmdText.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            var @operator = Operators.Parse($"=%{arg}%");
            return new Command(CmdType.Parameter, cmdText, matchedArguments, @operator);
        }

        private static readonly List<BuiltInParameter> NameLikeParameters = new List<BuiltInParameter>()
        {
            BuiltInParameter.ALL_MODEL_TYPE_NAME,
            BuiltInParameter.ALL_MODEL_MARK,
            BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM,
            BuiltInParameter.DATUM_TEXT
        };
    }
}