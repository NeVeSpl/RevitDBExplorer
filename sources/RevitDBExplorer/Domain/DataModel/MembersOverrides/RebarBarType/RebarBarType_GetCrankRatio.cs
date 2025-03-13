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
#if R2026_MIN
    internal class RebarBarType_GetCrankRatio : MemberAccessorByType<RebarBarType>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (RebarBarType x, ElementId i) => x.GetCrankRatio(i) ];


        protected override ReadResult Read(SnoopableContext context, RebarBarType rebarBarType) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Boolean), null),
            CanBeSnooped = true
        };
      

        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, RebarBarType rebarBarType)
        {            
            var crankTypes = RebarCrankTypeUtils.GetAllRebarCrankTypes(context.Document);
            foreach (var crankType in crankTypes)
            {
                yield return SnoopableObject.CreateInOutPair(context.Document, crankType, rebarBarType.GetCrankRatio(crankType));
            }           
        }
    }
#endif
}