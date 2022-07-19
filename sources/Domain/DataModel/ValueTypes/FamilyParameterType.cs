using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class FamilyParameterType : Base.ValueType<FamilyParameter>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new FamilyParameterType();
        }


        protected override bool CanBeSnoooped(FamilyParameter parameter) => false;
        protected override string ToLabel(FamilyParameter parameter)
        {
            return parameter?.Definition?.Name;
        }
    }
}