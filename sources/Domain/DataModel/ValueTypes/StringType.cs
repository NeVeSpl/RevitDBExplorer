using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class StringType : Base.ValueType<string>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new StringType();
        }


        protected override bool CanBeSnoooped(string stringValue) => false;
        protected override string ToLabel(string stringValue)
        {
            if (stringValue == "") return "<empty>";
            return stringValue;
        }
    }
}