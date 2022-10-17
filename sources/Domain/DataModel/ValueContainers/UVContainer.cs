using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Extensions.System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class UVContainer : Base.ValueContainer<UV>, IHaveToolTip
    {
        protected override bool CanBeSnoooped(UV uv) => false;
        protected override string ToLabel(UV uv)
        {
            return $"({uv.U}, {uv.V})";
        }

        public string ToolTip
        {
            get => $"{ToLabel(Value)}\n({Value.U.ToLengthDisplayString(Units)}, {Value.V.ToLengthDisplayString(Units)})";
        }
    }
}