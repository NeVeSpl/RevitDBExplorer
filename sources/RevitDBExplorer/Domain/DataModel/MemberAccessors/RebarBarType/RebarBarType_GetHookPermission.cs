using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class RebarBarType_GetHookPermission : MemberAccessorByType<RebarBarType>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (RebarBarType x, ElementId i) => x.GetHookPermission(i) ];


        public override ReadResult Read(SnoopableContext context, RebarBarType rebarBarType) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Boolean), null),
            CanBeSnooped = true
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, RebarBarType rebarBarType)
        {
            var hookTypes = new FilteredElementCollector(context.Document).OfClass(typeof(RebarHookType));
            foreach(var hookType in hookTypes)
            {
                yield return SnoopableObject.CreateInOutPair(context.Document, hookType, rebarBarType.GetHookPermission(hookType.Id));
            }           
        }
    }
}