using System;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class EnumType : Base.ValueType<System.Enum>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new EnumType();
        }

        
        protected override bool CanBeSnoooped(Enum enumValue) => false;
        protected override string ToLabel(Enum enumValue)
        {
            return $"{enumValue?.GetType()?.Name}.{enumValue}";
        }
    }
}