using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class BoundingBoxXYZHandler : TypeHandler<BoundingBoxXYZ>, IHaveToolTip<BoundingBoxXYZ>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, BoundingBoxXYZ box) => true;
        protected override string ToLabel(SnoopableContext context, BoundingBoxXYZ box)
        {
            return $"Min({box.Min.X:0.##}, {box.Min.Y:0.##}, {box.Min.Z:0.##}), Max({box.Max.X:0.##}, {box.Max.Y:0.##}, {box.Max.Z:0.##})";
        }


        public string GetToolTip(SnoopableContext context, BoundingBoxXYZ value)
        {
            var units = context.Document?.GetUnits();
            if (units == null) return null;

            return 
@$"{ToLabel(context, value)}
Min({value.Min.X.ToLengthDisplayString(units)}, {value.Min.Y.ToLengthDisplayString(units)}, {value.Min.Z.ToLengthDisplayString(units)}), Max({value.Max.X.ToLengthDisplayString(units)}, {value.Max.Y.ToLengthDisplayString(units)}, {value.Max.Z.ToLengthDisplayString(units)})
WDH({(value.Max.X - value.Min.X).ToLengthDisplayString(units)}, {(value.Max.Y - value.Min.Y).ToLengthDisplayString(units)}, {(value.Max.Z - value.Min.Z).ToLengthDisplayString(units)})";
        }

        protected override bool CanBeVisualized(SnoopableContext context, BoundingBoxXYZ value) => true;
        [CodeToString]
        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, BoundingBoxXYZ box)
        {
            if ((box.Max != null) && (box.Min != null))
            {
                var min = box.Transform.OfPoint(box.Min);
                var max = box.Transform.OfPoint(box.Max);                

                yield return new VisualizationItem("BoundingBoxXYZ", "Transform.OfPoint(box.Min)", new CrossDrawingVisual(min, VisualizationItem.StartColor));
                yield return new VisualizationItem("BoundingBoxXYZ", "Transform.OfPoint(box.Max)", new CrossDrawingVisual(max, VisualizationItem.EndColor));
            }
        }
    }
}