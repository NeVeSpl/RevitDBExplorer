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
    internal class RebarBarType_GetHookOffsetLength : MemberAccessorByType<RebarBarType>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (RebarBarType x, ElementId i) => x.GetHookOffsetLength(i) ];


        protected override ReadResult Read(SnoopableContext context, RebarBarType rebarBarType) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Double), null),
            CanBeSnooped = true
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, RebarBarType rebarBarType)
        {
            var hookTypes = new FilteredElementCollector(context.Document).OfClass(typeof(RebarHookType));
            var result = new List<SnoopableObject>();
            foreach (var hookType in hookTypes)
            {
                try
                {
                    result.Add(SnoopableObject.CreateInOutPair(context.Document, hookType, rebarBarType.GetHookOffsetLength(hookType.Id)));
                }
                catch (Exception ex)
                {
                    result.Add(SnoopableObject.CreateInOutPair(context.Document, hookType, ex));
                }
            }
            return result;
        }
    }
}