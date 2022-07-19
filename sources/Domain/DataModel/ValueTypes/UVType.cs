using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class UVType : Base.ValueType<UV>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new UVType();
        }


        protected override bool CanBeSnoooped(UV uv) => false;
        protected override string ToLabel(UV uv)
        {
            return $"({uv.U}, {uv.V})";
        }
    }
}