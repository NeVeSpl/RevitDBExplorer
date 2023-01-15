using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class BoundingBoxXYZHandler : TypeHandler<BoundingBoxXYZ>, IHaveToolTip<BoundingBoxXYZ>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, BoundingBoxXYZ box) => box is not null;
        protected override string ToLabel(SnoopableContext context, BoundingBoxXYZ box)
        {
            return $"Min({box.Min.X:0.##}, {box.Min.Y:0.##}, {box.Min.Z:0.##}), Max({box.Max.X:0.##}, {box.Max.Y:0.##}, {box.Max.Z:0.##})";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, BoundingBoxXYZ box)
        {
            yield return new SnoopableObject(document, box);
        }

        public string GetToolTip(SnoopableContext context, BoundingBoxXYZ value)
        {
            var units = context.Document.GetUnits();
            return 
@$"{ToLabel(context, value)}
Min({value.Min.X.ToLengthDisplayString(units)}, {value.Min.Y.ToLengthDisplayString(units)}, {value.Min.Z.ToLengthDisplayString(units)}), Max({value.Max.X.ToLengthDisplayString(units)}, {value.Max.Y.ToLengthDisplayString(units)}, {value.Max.Z.ToLengthDisplayString(units)})
WDH({(value.Max.X - value.Min.X).ToLengthDisplayString(units)}, {(value.Max.Y - value.Min.Y).ToLengthDisplayString(units)}, {(value.Max.Z - value.Min.Z).ToLengthDisplayString(units)})";
        }
    }
}