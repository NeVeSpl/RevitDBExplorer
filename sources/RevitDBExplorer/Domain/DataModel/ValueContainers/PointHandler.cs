using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Augmentations.RevitDatabaseVisualization.DrawingVisuals;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    class PointHandler : TypeHandler<Point>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Point point) => point is not null;
        protected override string ToLabel(SnoopableContext context, Point point) => point.GetType()?.GetCSharpName();
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, Point point)
        {
            yield return new SnoopableObject(context.Document, point);
        }


     
        private readonly static Color PointColor = new Color(80, 175, 228);

        protected override IEnumerable<DrawingVisual> GetVisualization(SnoopableContext context, Point point)
        {
            XYZ coord = point.Coord;
            yield return new CubeDrawingVisual(coord, PointColor);
        }
    }
}