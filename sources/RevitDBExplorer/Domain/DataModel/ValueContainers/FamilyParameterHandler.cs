using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class FamilyParameterHandler : TypeHandler<FamilyParameter>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, FamilyParameter parameter) => false;
        protected override string ToLabel(SnoopableContext context, FamilyParameter parameter)
        {
            return parameter?.Definition?.Name;
        }
    }
}