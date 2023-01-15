using System;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class XYZHandler : TypeHandler<XYZ>, IHaveToolTip<XYZ>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, XYZ xyz) => false;
        protected override string ToLabel(SnoopableContext context, XYZ xyz)
        {
            return $"({xyz.X}, {xyz.Y}, {xyz.Z})";
        }

        public string GetToolTip(SnoopableContext context, XYZ value)
        {
            var units = context.Document.GetUnits();
            return $"{ToLabel(context, value)}\n({value.X.ToLengthDisplayString(units)}, {value.Y.ToLengthDisplayString(units)},{value.Z.ToLengthDisplayString(units)})"; ;
        }
    }
}