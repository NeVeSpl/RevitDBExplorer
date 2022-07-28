using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Rebar_GetCenterlineCurves : MemberAccessorByType<Rebar>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Rebar x) => x.GetCenterlineCurves(false, true, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0); }      


        protected override bool CanBeSnoooped(Document document, Rebar rebar) => true;
        protected override string GetLabel(Document document, Rebar rebar) => $"[{nameof(Curve)} : {rebar.NumberOfBarPositions}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Rebar rebar)
        {
            for (int i = 0; i < rebar.NumberOfBarPositions; ++i)
            {
                var curves = rebar.GetCenterlineCurves(false, true, false, MultiplanarOption.IncludeOnlyPlanarCurves, i);
                yield return new SnoopableObject(document, i, curves.Select(x => new SnoopableObject(document, x))) { NamePrefix = $"barPositionIndex:" };
            }           
        }
    }
}