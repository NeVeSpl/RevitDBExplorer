using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class RebarBarType_GetHookOffsetLength : MemberAccessorByType<RebarBarType>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (RebarBarType x, ElementId i) => x.GetHookOffsetLength(i); }      


        protected override bool CanBeSnoooped(Document document, RebarBarType rebarBarType) => true;
        protected override string GetLabel(Document document, RebarBarType rebarBarType) => $"[{nameof(Double)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, RebarBarType rebarBarType)
        {
            var hookTypes = new FilteredElementCollector(document).OfClass(typeof(RebarHookType));
            var result = new List<SnoopableObject>();
            foreach(var hookType in hookTypes)
            {
                try
                {
                    result.Add(SnoopableObject.CreateInOutPair(document, hookType, rebarBarType.GetHookOffsetLength(hookType.Id)));
                }
                catch(Exception ex)
                {
                    result.Add(SnoopableObject.CreateInOutPair(document, hookType, ex));
                }
            }
            return result;
        }
    }
}