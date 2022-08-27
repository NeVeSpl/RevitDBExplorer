using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
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