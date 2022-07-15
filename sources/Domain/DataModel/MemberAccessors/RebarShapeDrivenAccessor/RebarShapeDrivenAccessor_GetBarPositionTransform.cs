using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class RebarShapeDrivenAccessor_GetBarPositionTransform : MemberAccessorByType<RebarShapeDrivenAccessor>, IHaveFactoryMethod
    {
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (RebarShapeDrivenAccessor x) => x.GetBarPositionTransform(13); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new RebarShapeDrivenAccessor_GetBarPositionTransform();


        protected override bool CanBeSnoooped(Document document, RebarShapeDrivenAccessor rebar) => true;
        protected override string GetLabel(Document document, RebarShapeDrivenAccessor rebar) => "[Transform]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, RebarShapeDrivenAccessor rebar)
        {
            for (int i = 0; i < rebar.ArrayLength; ++i)
            {
                var transform = rebar.GetBarPositionTransform(i);
                yield return new SnoopableObject(null, document, $"barPositionIndex: {i}", new[] { new SnoopableObject(transform, document) });
            }           
        }
    }
}
