using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitExplorer.Visualizations.DrawingVisuals;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class XYZHandler : TypeHandler<XYZ>, IHaveToolTip<XYZ>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, XYZ xyz) => true;
        protected override string ToLabel(SnoopableContext context, XYZ xyz)
        {
            return $"({xyz.X}, {xyz.Y}, {xyz.Z})";
        }

        public string GetToolTip(SnoopableContext context, XYZ xyz)
        {
            var units = context.Document?.GetUnits();
            if (units == null) return null;

            return $"{ToLabel(context, xyz)}\n({xyz.X.ToLengthDisplayString(units)}, {xyz.Y.ToLengthDisplayString(units)},{xyz.Z.ToLengthDisplayString(units)})"; ;
        }

        protected override bool CanBeVisualized(SnoopableContext context, XYZ xyz) => true;
        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, XYZ xyz)
        {
            yield return new VisualizationItem("XYZ", "*", new CrossDrawingVisual(xyz, VisualizationItem.PointColor));
        }
    }
}