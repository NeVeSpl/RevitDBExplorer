using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class ParameterCmdFactory : CommandFactory<ParameterMatch>
    {
        public override IAutocompleteItem GetAutocompleteItem() => new AutocompleteItem("p:[parametr] = [value] - ", "p: ");

        public override IEnumerable<string> GetClassifiers()
        {
            yield return "p";
        }
        public override IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public override bool CanRecognizeArgument(string argument)
        {
            if (argument.StartsWith(nameof(BuiltInParameter), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (Operators.DoesContainAnyValidOperator(argument))
            {
                return true;
            }
            return false;
        }


        public override ICommand Create(string cmdText, IList<ILookupResult> arguments)
        {
            var @operator = Operators.Parse(cmdText);
            return new Command(CmdType.Parameter, cmdText, arguments, @operator);
        }
        public override IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            var leftSide = Operators.GetLeftSideOfOperator(argument);
            var bareArgument = leftSide.RemovePrefix(nameof(BuiltInParameter));
            return FuzzySearchEngine.Lookup(bareArgument, FuzzySearchEngine.LookFor.Parameter);
        }
    }



}