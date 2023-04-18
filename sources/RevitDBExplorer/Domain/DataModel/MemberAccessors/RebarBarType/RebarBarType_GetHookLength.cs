using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class RebarBarType_GetHookLength : MemberAccessorByType<RebarBarType>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (RebarBarType x, ElementId i) => x.GetHookLength(i); }      


        protected override bool CanBeSnoooped(Document document, RebarBarType rebarBarType) => true;
        protected override string GetLabel(Document document, RebarBarType rebarBarType) => $"[{nameof(Double)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, RebarBarType rebarBarType)
        {
            var hookTypes = new FilteredElementCollector(document).OfClass(typeof(RebarHookType));
            foreach(var hookType in hookTypes)
            {
                yield return SnoopableObject.CreateInOutPair(document, hookType, rebarBarType.GetHookLength(hookType.Id));
            }           
        }
    }
}