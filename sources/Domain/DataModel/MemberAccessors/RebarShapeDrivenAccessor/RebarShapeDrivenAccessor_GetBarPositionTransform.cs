using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class RebarShapeDrivenAccessor_GetBarPositionTransform : MemberAccessorByType<RebarShapeDrivenAccessor>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (RebarShapeDrivenAccessor x) => x.GetBarPositionTransform(13); }       


        protected override bool CanBeSnoooped(Document document, RebarShapeDrivenAccessor rebar) => false;
        protected override string GetLabel(Document document, RebarShapeDrivenAccessor rebar) => $"[{nameof(Transform)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, RebarShapeDrivenAccessor rebar)
        {
            for (int i = 0; i < 0; ++i)
            {
                var transform = rebar.GetBarPositionTransform(i);
                yield return SnoopableObject.CreateInOutPair(document, i, transform, "barPositionIndex:");
            }           
        }
    }
}