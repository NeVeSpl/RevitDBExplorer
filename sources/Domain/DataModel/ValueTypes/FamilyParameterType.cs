using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class FamilyParameterType : Base.ValueType<FamilyParameter>
    {
        protected override bool CanBeSnoooped(FamilyParameter parameter) => false;
        protected override string ToLabel(FamilyParameter parameter)
        {
            return parameter?.Definition?.Name;
        }
    }
}