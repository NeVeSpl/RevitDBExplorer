using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class VisibleInViewMatch : LookupResult<View>
    {
        public VisibleInViewMatch(View view, double levensteinScore) : base(view, levensteinScore)
        {
            CmdType = CmdType.ActiveView;
            Name = $"{view.Id}";
            Label = view.Name;
        }
    }
}
