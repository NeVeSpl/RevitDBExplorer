using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class BoundarySegmentHandler : TypeHandler<BoundarySegment>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, BoundarySegment boundarySegment) => boundarySegment is not null;

        protected override string ToLabel(SnoopableContext context, BoundarySegment boundarySegment)
        {
            return $"ID: {boundarySegment.ElementId}, {boundarySegment.GetCurve()?.Length} ft"; ;
        }
    }
}