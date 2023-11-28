using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class SpatialElement_GetBoundarySegments : MemberAccessorByType<SpatialElement>, ICanCreateMemberAccessor
    {
        public IEnumerable<LambdaExpression> GetHandledMembers() { yield return (SpatialElement x) => x.GetBoundarySegments(null); }


        protected override bool CanBeSnoooped(Document document, SpatialElement value) => true;

        protected override string GetLabel(Document document, SpatialElement value)
        {
            return "[[BoundarySegment]]";
        }

        protected override IEnumerable<SnoopableObject> Snooop(Document document, SpatialElement element)
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
                yield return SnoopableObject.CreateKeyValuePair(document, option, element.GetBoundarySegments(option), "options");
            }
        }
    }
}
