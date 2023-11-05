using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Augmentations.RevitDatabaseVisualization.DrawingVisuals;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class SolidHandler : TypeHandler<Solid>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Solid solid) => solid is not null;
        protected override string ToLabel(SnoopableContext context, Solid solid) => solid.GetType()?.GetCSharpName();
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, Solid solid)
        {
            yield return new SnoopableObject(context.Document, solid);
        }



        private readonly static Color SolidColor = new Color(80, 175, 228);

        protected override IEnumerable<DrawingVisual> GetVisualization(SnoopableContext context, Solid solid)
        {
            yield return new SolidDrawingVisual(solid, SolidColor);
        }
    }
}
