using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
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