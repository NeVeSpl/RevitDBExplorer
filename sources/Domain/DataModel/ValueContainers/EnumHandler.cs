using System;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class EnumHandler<TEnumType> : TypeHandler<TEnumType> where TEnumType : System.Enum
    {
        protected override bool CanBeSnoooped(SnoopableContext context, TEnumType enumValue) => false;
        protected override string ToLabel(SnoopableContext context, TEnumType enumValue)
        {
            return $"{enumValue?.GetType()?.Name}.{enumValue}";
        }
    }
}