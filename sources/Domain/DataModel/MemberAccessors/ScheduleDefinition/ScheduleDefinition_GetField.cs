using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class ScheduleDefinition_GetField : MemberAccessorByType<ScheduleDefinition>, IHaveFactoryMethod
    {       
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (ScheduleDefinition x) => x.GetField(7); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new ScheduleDefinition_GetField();


        protected override bool CanBeSnoooped(Document document, ScheduleDefinition scheduleDefinition)
        {
            bool canBesnooped = scheduleDefinition is not null && scheduleDefinition.GetFieldCount() > 0;
            return canBesnooped;
        }
        protected override string GetLabel(Document document, ScheduleDefinition scheduleDefinition) => "[ScheduleField]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ScheduleDefinition scheduleDefinition)
        {
            for (var i = 0; i < scheduleDefinition.GetFieldCount(); i++)
            {
                var field = scheduleDefinition.GetField(i);
                yield return new SnoopableObject(field, document);
            }
        }
    }
}