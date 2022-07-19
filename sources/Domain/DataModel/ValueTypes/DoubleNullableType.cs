using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class DoubleNullableType : Base.ValueType<double?>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new DoubleNullableType();
        }


        protected override bool CanBeSnoooped(double? doubleValue) => false;
        protected override string ToLabel(double? doubleValue)
        {
            return doubleValue.ToString();
        }
    }
}