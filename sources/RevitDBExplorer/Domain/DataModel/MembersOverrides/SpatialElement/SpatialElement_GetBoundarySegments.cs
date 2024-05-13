using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class SpatialElement_GetBoundarySegments : MemberAccessorByType<SpatialElement>, ICanCreateMemberAccessor
    {
        public IEnumerable<LambdaExpression> GetHandledMembers() => [ (SpatialElement x) => x.GetBoundarySegments(null) ];


        protected override ReadResult Read(SnoopableContext context, SpatialElement spatialElement) => new()
        {
            Label = "[[BoundarySegment]]",
            CanBeSnooped = true
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, SpatialElement element)
        {
            var options = new[]
            {
                new SpatialElementBoundaryOptions() { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center, StoreFreeBoundaryFaces = true },
                new SpatialElementBoundaryOptions() { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreBoundary, StoreFreeBoundaryFaces = true },
                new SpatialElementBoundaryOptions() { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish, StoreFreeBoundaryFaces = true },
                new SpatialElementBoundaryOptions() { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreCenter, StoreFreeBoundaryFaces = true },
                new SpatialElementBoundaryOptions() { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center, StoreFreeBoundaryFaces = false },
                new SpatialElementBoundaryOptions() { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreBoundary, StoreFreeBoundaryFaces = false },
                new SpatialElementBoundaryOptions() { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish, StoreFreeBoundaryFaces = false },
                new SpatialElementBoundaryOptions() { SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreCenter, StoreFreeBoundaryFaces = false },
            };

            foreach (var option in options)
            {
                yield return SnoopableObject.CreateKeyValuePair(context.Document, option, element.GetBoundarySegments(option), "options");
            }
        }
    }
}
