using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Rebar_GetOverridableHookParameters : MemberAccessorByType<Rebar>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Rebar x, ISet<ElementId> s1, ISet<ElementId> s2, ISet<ElementId> s3, ISet<ElementId> s4) => x.GetOverridableHookParameters(out s1, out s2, out s3, out s4); }


        protected override bool CanBeSnoooped(Document document, Rebar rebar) => rebar.IsHookLengthOverrideEnabled();
        protected override string GetLabel(Document document, Rebar rebar) => $"[{nameof(ElementId)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Rebar rebar)
        {
            ISet<ElementId> startHookLengthPrameters = null;
            ISet<ElementId> startHookTangentLengthParameters = null;
            ISet<ElementId> endHookLengthParameters = null;
            ISet<ElementId> endHookTangentLengthParameters = null;

            rebar.GetOverridableHookParameters(out startHookLengthPrameters, out startHookTangentLengthParameters, out endHookLengthParameters, out endHookTangentLengthParameters);

            yield return new SnoopableObject(document, "startHookLengthPrameters", startHookLengthPrameters.Select(x => new SnoopableObject(document, x)));
            yield return new SnoopableObject(document, "startHookTangentLengthParameters", startHookTangentLengthParameters.Select(x => new SnoopableObject(document, x)));
            yield return new SnoopableObject(document, "endHookLengthParameters", endHookLengthParameters.Select(x => new SnoopableObject(document, x)));
            yield return new SnoopableObject(document, "endHookTangentLengthParameters", endHookTangentLengthParameters.Select(x => new SnoopableObject(document, x)));
        }
    }
}
