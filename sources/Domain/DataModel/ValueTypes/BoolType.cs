using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class BoolType : Base.ValueType<bool>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new BoolType();
        }


        protected override bool CanBeSnoooped(bool boolean) => false;
        protected override string ToLabel(bool boolean) => boolean.ToString();        
    }
}