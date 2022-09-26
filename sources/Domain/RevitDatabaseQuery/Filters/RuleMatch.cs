using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class RuleMatch : LookupResult<ElementId>
    {
        public RuleMatch(ElementId filterId, double levensteinScore, string name) : base(filterId, levensteinScore)
        {
            CmdType = CmdType.RuleBasedFilter;
            Name = $"new ElementId({filterId.IntegerValue})";
            Label = name;
        }
    }
}
