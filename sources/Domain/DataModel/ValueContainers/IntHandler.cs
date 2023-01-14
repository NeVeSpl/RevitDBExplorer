using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class IntHandler : TypeHandler<int>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, int intValue) => false;
        protected override string ToLabel(SnoopableContext context, int intValue)
        {
            return intValue.ToString();
        }
    }
}