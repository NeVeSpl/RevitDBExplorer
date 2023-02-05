using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class NameCmdFactory : ICommandFactory
    {
        public static readonly NameCmdFactory Instance = new NameCmdFactory();
        public IAutocompleteItem GetAutocompleteItem() => new AutocompleteItem("n:[text] - wildcard search for a given text", "n: ");

        public Type MatchType => null;

        public IEnumerable<string> GetClassifiers()
        {
            yield return "n";
            yield return "name";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            return false;
        }

        public ICommand Create(string cmdText, IList<ILookupResult> arguments)
        {
            var arg = cmdText.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            var @operator = Operators.Parse($"=%{arg}%");
            return new Command(CmdType.Parameter, cmdText, arguments, @operator);
        }
        public IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            var matchedArguments = NameLikeParameters.Select(x => new ParameterMatch(x, 1)).ToArray();
            return matchedArguments;
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