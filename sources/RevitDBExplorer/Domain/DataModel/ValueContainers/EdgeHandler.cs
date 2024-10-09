using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class EdgeHandler : TypeHandler<Edge>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Edge edge) => true;
        protected override string ToLabel(SnoopableContext context, Edge edge) => edge.GetType()?.GetCSharpName();


        protected override bool CanBeVisualized(SnoopableContext context, Edge edge) => true;
        [CodeToString]
        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, Edge edge)
        {
            var curve = edge.AsCurve();

            if (curve is Line line)
            {
                //yield return new VisualizationItem("Edge", "AsCurve().Origin", new CubeDrawingVisual(line.Origin, VisualizationItem.Accent1Color));
                //yield return new VisualizationItem("Edge", "AsCurve().Direction", new ArrowDrawingVisual(line.Origin, line.Direction, VisualizationItem.Accent2Color));
            }

            if (curve.IsBound)
            {
                var startPoint = curve.GetEndPoint(0);
                var endPoint = curve.GetEndPoint(1);

                yield return new VisualizationItem("Edge", "start - AsCurve().GetEndPoint(0)", new CubeDrawingVisual(startPoint, VisualizationItem.StartColor));
                yield return new VisualizationItem("Edge", "end - AsCurve().GetEndPoint(1)", new CubeDrawingVisual(endPoint, VisualizationItem.EndColor));
            }
            yield return new VisualizationItem("Edge", "AsCurve()", new CurveDrawingVisual(curve, VisualizationItem.CurveColor));
        }
    }
}
