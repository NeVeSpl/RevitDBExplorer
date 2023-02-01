using System.Collections.Generic;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class LevelCmdFactory : CommandFactory<LevelMatch>
    {
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
}