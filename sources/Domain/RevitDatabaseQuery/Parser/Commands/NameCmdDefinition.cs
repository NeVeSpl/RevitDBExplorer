using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class NameCmdDefinition : ICommandDefinition
    {
        public static readonly NameCmdDefinition Instance = new NameCmdDefinition();
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("n: ", "n:[text]", "wildcard search for a given text", AutocompleteItemGroups.Commands);
        private readonly DataBucket<ParameterArgument> dataBucket = new DataBucket<ParameterArgument>(0.69);

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
            var matchedArguments = NameLikeParameters.Select(x => new ParameterArgument(x)).ToArray();
          
            var @operator = Operators.Parse($"=%{argument}%");
            return new ParameterCmd(cmdText, dataBucket.CreateMatch(matchedArguments), @operator);
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