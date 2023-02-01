using System.Collections.Generic;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class RoomCmdFactory : CommandFactory<RoomMatch>
    {
        public override IEnumerable<string> GetClassifiers()
        {
            yield return "r";
            yield return "room";        
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
            return new Command(CmdType.Room, cmdText, arguments, null);
        }
        public override IEnumerable<ILookupResult> ParseArgument(string argument)
        {
            return FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.Room);
        }
    }
}