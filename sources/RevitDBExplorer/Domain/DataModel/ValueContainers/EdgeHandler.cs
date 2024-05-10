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

   

        private readonly static Color StartColor = new Color(0, 255, 0);
        private readonly static Color EndColor = new Color(255, 0, 0);
        private readonly static Color CurveColor = new Color(80, 175, 228);

        protected override bool CanBeVisualized(SnoopableContext context, Edge edge) => true;
        [CodeToString]
        protected override IEnumerable<DrawingVisual> GetVisualization(SnoopableContext context, Edge edge)
        {
            var curve = edge.AsCurve();

            if (curve.IsBound)
            {
                var startPoint = curve.GetEndPoint(0);
                var endPoint = curve.GetEndPoint(1);

                yield return new CubeDrawingVisual(startPoint, StartColor);
                yield return new CubeDrawingVisual(endPoint, EndColor);
            }
            yield return new CurveDrawingVisual(curve, CurveColor);
        }
    }
}
