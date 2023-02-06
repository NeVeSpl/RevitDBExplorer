using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class ElementIdCmdFactory : CommandFactory<ElementIdMatch>
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("i: ", "i:[number] ", "select elements with given id");

        public override IAutocompleteItem GetAutocompleteItem() => AutocompleteItem;
      

        public override IEnumerable<string> GetClassifiers()
        {
            yield return "i";
            yield return "id";
            yield return "ids";
        }
        public override IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public override bool CanRecognizeArgument(string argument)
        {
            if (long.TryParse(argument, out long longValue))
            {
                return true;
            }
            return false;
        }


        public override ICommand Create(string cmdText, IList<ILookupResult> arguments)
        {
            return new Command(CmdType.ElementId, cmdText, arguments, null) { IsBasedOnQuickFilter = true };
        }
        public override IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            return FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.ElementId);
        }
    }


    internal class ElementIdMatch : LookupResult<ElementId>
    {
        public ElementIdMatch(ElementId value, double levensteinScore) : base(value, levensteinScore)
        {
            CmdType = CmdType.ElementId;
            Name = $"new ElementId({value})";
            Label = value.Value().ToString();
        }
    }
}