using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitExplorer.Visualizations.DrawingVisuals;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class SolidHandler : TypeHandler<Solid>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Solid solid) => true;
        protected override string ToLabel(SnoopableContext context, Solid solid) => solid.GetType()?.GetCSharpName();
     



        private readonly static Color SolidColor = new Color(80, 175, 228);

        protected override bool CanBeVisualized(SnoopableContext context, Solid solid) => true;
        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, Solid solid)
        {
            yield return new VisualizationItem("Solid", "*", new SolidDrawingVisual(solid, VisualizationItem.SolidColor));
        }
    }
}
