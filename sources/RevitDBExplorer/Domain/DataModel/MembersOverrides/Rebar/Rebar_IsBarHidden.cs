using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Rebar_IsBarHidden : MemberAccessorByType<Rebar>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (Rebar x, View v) => x.IsBarHidden(v, 7) ];


        protected override ReadResult Read(SnoopableContext context, Rebar rebar) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Boolean), rebar.NumberOfBarPositions),
            CanBeSnooped = true
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Rebar rebar)
        {
            for (int i = 0; i < rebar.NumberOfBarPositions; ++i)
            {
                var result = rebar.IsBarHidden(context.Document.ActiveView, i);
                yield return SnoopableObject.CreateInOutPair(context.Document, i, result, "barIndex:");
            }
        }
    }
}