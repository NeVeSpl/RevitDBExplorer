using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class ParameterType : Base.ValueType<Parameter>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new ParameterType();
        }


        protected override bool CanBeSnoooped(Parameter parameter) => false;
        protected override string ToLabel(Parameter parameter)
        {
            return parameter?.Definition?.Name;
        }
    }
}
