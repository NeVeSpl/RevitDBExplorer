using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class FaceHandler : TypeHandler<Face>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Face face) => true;
        protected override string ToLabel(SnoopableContext context, Face face) => face.GetType()?.GetCSharpName();
               
        

        protected override bool CanBeVisualized(SnoopableContext context, Face face) => true;
        [CodeToString]
        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, Face face)
        {            
            if (face is PlanarFace planarFace)
            {
                yield return new VisualizationItem("PlanarFace", "*", new FaceDrawingVisual(face, VisualizationItem.FaceColor));
                yield return new VisualizationItem("PlanarFace", "Normal", new ArrowDrawingVisual(planarFace.Origin, planarFace.FaceNormal, VisualizationItem.NormalColor));
                yield return new VisualizationItem("PlanarFace", "XVector", new ArrowDrawingVisual(planarFace.Origin, planarFace.XVector, VisualizationItem.XAxisColor));
                yield return new VisualizationItem("PlanarFace", "YVector", new ArrowDrawingVisual(planarFace.Origin, planarFace.YVector, VisualizationItem.YAxisColor));
            }
        }
    }
}