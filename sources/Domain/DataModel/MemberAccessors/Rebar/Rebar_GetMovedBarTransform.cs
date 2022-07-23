using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Rebar_GetMovedBarTransform : MemberAccessorByType<Rebar>, ICanCreateMemberAccessor
    {
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (Rebar x) => x.GetMovedBarTransform(0); } }
        IMemberAccessor ICanCreateMemberAccessor.Create() => new Rebar_GetMovedBarTransform();


        protected override bool CanBeSnoooped(Document document, Rebar rebar) => true;
        protected override string GetLabel(Document document, Rebar rebar) => $"[{nameof(Transform)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Rebar rebar)
        {
            for (int i = 0; i < rebar.NumberOfBarPositions; ++i)
            {
                Transform transform = rebar.GetMovedBarTransform(i);
                yield return SnoopableObject.CreateInOutPair(document, i, transform, "barPositionIndex:");
            }           
        }
    }
}