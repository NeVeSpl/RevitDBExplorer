using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Rebar_DoesBarExistAtPosition : MemberAccessorByType<Rebar>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Rebar x, View v) => x.DoesBarExistAtPosition(7); }        


        protected override bool CanBeSnoooped(Document document, Rebar rebar) => true;
        protected override string GetLabel(Document document, Rebar rebar) => $"[{nameof(Boolean)} : {rebar.NumberOfBarPositions}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Rebar rebar)
        {
            for (int i = 0; i < rebar.NumberOfBarPositions; ++i)
            {
                var result = rebar.DoesBarExistAtPosition(i);
                yield return SnoopableObject.CreateInOutPair(document, i, result, "barPosition:");
            }
        }
    }
}