using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class ValueTypeHandler<T> : TypeHandler<T> where T : struct
    {
        protected override bool CanBeSnoooped(SnoopableContext context, T value) => false;

        protected override string ToLabel(SnoopableContext context, T value) => value.ToString();
    }
}