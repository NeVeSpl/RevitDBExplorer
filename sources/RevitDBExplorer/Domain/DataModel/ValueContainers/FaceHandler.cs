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

        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, Face face)
        {
            yield return new SnoopableObject(context.Document, face);
        }


        private readonly static Color FaceColor = new Color(80, 175, 228);

        protected override bool CanBeVisualized(SnoopableContext context, Face face) => true;
        [CodeToString]
        protected override IEnumerable<DrawingVisual> GetVisualization(SnoopableContext context, Face face)
        {
            yield return new FaceDrawingVisual(face, FaceColor);
        }
    }
}
