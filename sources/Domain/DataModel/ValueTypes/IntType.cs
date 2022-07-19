using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class IntType : Base.ValueType<int>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new IntType();
        }


        protected override bool CanBeSnoooped(int intValue) => false;
        protected override string ToLabel(int intValue)
        {
            return intValue.ToString();
        }
    }
}