using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Rebar_GetCenterlineCurves : MemberAccessorByType<Rebar>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (Rebar x) => x.GetCenterlineCurves(false, true, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0) ];


        public Rebar_GetCenterlineCurves()
        {
            DefaultInvocation.Syntax = "item.GetCenterlineCurves(false, true, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0)";
        }


        protected override ReadResult Read(SnoopableContext context, Rebar rebar) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Curve), rebar.NumberOfBarPositions),
            CanBeSnooped = true
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Rebar rebar)
        {
            for (int i = 0; i < rebar.NumberOfBarPositions; ++i)
            {
                var curves = rebar.GetCenterlineCurves(false, true, false, MultiplanarOption.IncludeOnlyPlanarCurves, i);
                yield return new SnoopableObject(context.Document, i, curves.Select(x => new SnoopableObject(context.Document, x))) { NamePrefix = $"barPositionIndex:" };
            }           
        }
    }
}