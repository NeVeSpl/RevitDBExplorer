using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class LevelCmdFactory : CommandFactory<LevelMatch>
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("l: ", "l:[level]", "select elements from a given level");

        public override IAutocompleteItem GetAutocompleteItem() => AutocompleteItem;
      

        public override IEnumerable<string> GetClassifiers()
        {
            yield return "l";
            yield return "lvl";
            yield return "level";         
        }
        public override IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public override bool CanRecognizeArgument(string argument)
        {           
            return false;
        }


        public override ICommand Create(string cmdText, IList<ILookupResult> arguments)
        {
            return new Command(CmdType.Level, cmdText, arguments, null);
        }
        public override IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            return FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.Level);
        }
    }


    internal class LevelMatch : LookupResult<ElementId>
    {
        public LevelMatch(ElementId levelId, double levensteinScore, string name) : base(levelId, levensteinScore)
        {
            CmdType = CmdType.Level;
            Name = $"new ElementId({levelId})";
            Label = name;
        }
    }
}