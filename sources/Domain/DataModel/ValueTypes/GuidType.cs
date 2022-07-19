using System;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class GuidType : Base.ValueType<Guid>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new GuidType();
        }


        protected override bool CanBeSnoooped(Guid guid) => false;
        protected override string ToLabel(Guid guid)
        {
            return guid.ToString();
        }
    }
}