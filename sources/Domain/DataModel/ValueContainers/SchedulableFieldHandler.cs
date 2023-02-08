using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    class SchedulableFieldHandler : TypeHandler<SchedulableField>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, SchedulableField value) => value is not null;

        protected override string ToLabel(SnoopableContext context, SchedulableField value)
        {
            return $"SchedulableField: {value.GetName(context.Document)}";
        }
    }
}
