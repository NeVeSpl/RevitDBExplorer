using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class XYZType : Base.ValueType<XYZ>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new XYZType();
        }


        protected override bool CanBeSnoooped(XYZ xyz) => false;
        protected override string ToLabel(XYZ xyz)
        {
            return $"({xyz.X}, {xyz.Y}, {xyz.Z})";
        }
    }
}