using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class TransformHandler : TypeHandler<Transform>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Transform transform) => true;

        protected override string ToLabel(SnoopableContext context, Transform transform)
        {
            string id = "";
            if (transform.IsIdentity)
            {
                id = "Identity";
            }
            if (transform.IsTranslation)
            {
                id = "Translation";
            }

            return $"Transform: {id}";
        }


        protected override bool CanBeVisualized(SnoopableContext context, Transform transform) => true;


        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, Transform transform)
        {   
            var transformedOrgin = transform.OfPoint(XYZ.Zero);
            yield return new VisualizationItem("Transform", "*", new VectorDrawingVisual(XYZ.Zero, transformedOrgin, VisualizationItem.Accent3Color));                     
        }
    }
}