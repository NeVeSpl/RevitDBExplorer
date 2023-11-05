using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Augmentations.RevitDatabaseVisualization.DrawingVisuals;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class FaceHandler : TypeHandler<Face>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Face face) => face is not null;
        protected override string ToLabel(SnoopableContext context, Face face) => face.GetType()?.GetCSharpName();
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, Face face)
        {
            yield return new SnoopableObject(context.Document, face);
        }


        private readonly static Color FaceColor = new Color(80, 175, 228);

        protected override IEnumerable<DrawingVisual> GetVisualization(SnoopableContext context, Face face)
        {
            yield return new FaceDrawingVisual(face, FaceColor);
        }
    }
}
