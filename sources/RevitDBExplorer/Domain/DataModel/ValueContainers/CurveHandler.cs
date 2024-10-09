using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class CurveHandler : TypeHandler<Curve>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Curve curve) => true;
        protected override string ToLabel(SnoopableContext context, Curve curve) => curve.GetType()?.GetCSharpName();       

        
        protected override bool CanBeVisualized(SnoopableContext context, Curve curve) => true;
        [CodeToString]
        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, Curve curve)
        {
            if (curve is Line line)
            {
                yield return new VisualizationItem("Line", "Origin", new CubeDrawingVisual(line.Origin, VisualizationItem.Accent1Color));
                yield return new VisualizationItem("Line", "Direction", new ArrowDrawingVisual(line.Origin, line.Direction, VisualizationItem.Accent2Color));
            }

            if (curve.IsBound)
            {
                var startPoint = curve.GetEndPoint(0);
                var endPoint = curve.GetEndPoint(1);

                yield return new VisualizationItem("Curve", "start - curve.GetEndPoint(0)", new CubeDrawingVisual(startPoint, VisualizationItem.StartColor));
                yield return new VisualizationItem("Curve", "end - curve.GetEndPoint(1)", new CubeDrawingVisual(endPoint, VisualizationItem.EndColor));
            }
            yield return new VisualizationItem("Curve", "*", new CurveDrawingVisual(curve, VisualizationItem.CurveColor));
        }
    }
}
