using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class RoomCmdFactory : CommandFactory<RoomMatch>
    {
        public override IAutocompleteItem GetAutocompleteItem() => new AutocompleteItem("r:[room] - select elements from a given room", "r: ");

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


    internal class RoomMatch : LookupResult<ElementId>
    {
        public RoomMatch(ElementId roomId, double levensteinScore, string name) : base(roomId, levensteinScore)
        {
            CmdType = CmdType.Room;
            Name = $"{name}.ClosedShell";
            Label = name;
        }
    }
}