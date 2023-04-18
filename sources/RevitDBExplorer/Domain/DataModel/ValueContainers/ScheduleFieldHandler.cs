using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class ScheduleFieldHandler : TypeHandler<ScheduleField>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, ScheduleField value) => value is not null;
        protected override string ToLabel(SnoopableContext context, ScheduleField field)
        {
            return $"[{field.FieldIndex}] {field.ColumnHeading}";
        }
    }
}