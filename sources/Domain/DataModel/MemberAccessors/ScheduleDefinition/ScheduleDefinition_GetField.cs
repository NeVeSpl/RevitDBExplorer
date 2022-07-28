using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class ScheduleDefinition_GetField : MemberAccessorByType<ScheduleDefinition>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (ScheduleDefinition x) => x.GetField(7); }  


        protected override bool CanBeSnoooped(Document document, ScheduleDefinition scheduleDefinition)
        {
            bool canBesnooped = scheduleDefinition.GetFieldCount() > 0;
            return canBesnooped;
        }
        protected override string GetLabel(Document document, ScheduleDefinition scheduleDefinition) => $"Fields : {scheduleDefinition.GetFieldCount()}";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ScheduleDefinition scheduleDefinition)
        {
            for (var i = 0; i < scheduleDefinition.GetFieldCount(); i++)
            {
                var field = scheduleDefinition.GetField(i);
                yield return new SnoopableObject(document, field);
            }
        }
    }
}