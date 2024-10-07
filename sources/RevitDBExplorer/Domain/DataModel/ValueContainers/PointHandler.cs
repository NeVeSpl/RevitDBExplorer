using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitExplorer.Visualizations.DrawingVisuals;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    class PointHandler : TypeHandler<Point>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Point point) => true;
        protected override string ToLabel(SnoopableContext context, Point point) => point.GetType()?.GetCSharpName();
      


     
        private readonly static Color PointColor = new Color(80, 175, 228);

        protected override bool CanBeVisualized(SnoopableContext context, Point point) => true;
        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, Point point)
        {
            XYZ coord = point.Coord;
            yield return new VisualizationItem("Point", "Coord", new CubeDrawingVisual(coord, PointColor));
        }
    }
}