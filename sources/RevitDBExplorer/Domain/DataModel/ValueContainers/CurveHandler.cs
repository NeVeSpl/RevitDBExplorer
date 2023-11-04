using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Augmentations.RevitDatabaseVisualization.DrawingVisuals;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class CurveHandler : TypeHandler<Curve>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Curve curve) => curve is not null;
        protected override string ToLabel(SnoopableContext context, Curve curve) => curve.GetType()?.GetCSharpName();
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, Curve curve)
        {           
            yield return new SnoopableObject(context.Document, curve);
        }


        private readonly static Color StartColor = new Color(0, 255, 0);
        private readonly static Color EndColor = new Color(255, 0, 0);

        protected override IEnumerable<DrawingVisual> GetVisualization(SnoopableContext context, Curve curve)
        {  
            var startPoint = curve.GetEndPoint(0);
            var endPoint = curve.GetEndPoint(1);
            
            yield return new Cube(startPoint, StartColor);
            yield return new Cube(endPoint, EndColor);            
        }
    }
}
