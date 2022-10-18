using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Extensions.System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class XYZContainer : Base.ValueContainer<XYZ>, IHaveToolTip
    {
        protected override bool CanBeSnoooped(XYZ xyz) => false;
        protected override string ToLabel(XYZ xyz)
        {
            return $"({xyz.X}, {xyz.Y}, {xyz.Z})";
        }

        public string ToolTip
        {
            get => $"{ToLabel(Value)}\n({Value.X.ToLengthDisplayString(Units)}, {Value.Y.ToLengthDisplayString(Units)},{Value.Z.ToLengthDisplayString(Units)})";
        }
    }
}