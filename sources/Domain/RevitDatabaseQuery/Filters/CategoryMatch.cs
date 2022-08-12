using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Filters
{
    internal class CategoryMatch : LookupResult<BuiltInCategory>
    {
        public CategoryMatch(BuiltInCategory value, double levensteinScore) : base(value, levensteinScore)
        {
            CmdType = CmdType.Category;
            Name = $"BuiltInCategory.{value}";
            Label = LabelUtils.GetLabelFor(value);
        }
    }
}