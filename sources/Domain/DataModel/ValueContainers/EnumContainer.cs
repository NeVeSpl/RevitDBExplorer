using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class EnumContainer : Base.ValueContainer<System.Enum>
    {
        protected override bool CanBeSnoooped(Enum enumValue) => false;
        protected override string ToLabel(Enum enumValue)
        {
            return $"{enumValue?.GetType()?.Name}.{enumValue}";
        }
    }
}