using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal class ScheduleFieldType : Base.ValueType<ScheduleField>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create() => new ScheduleFieldType();
        

        protected override bool CanBeSnoooped(ScheduleField value) => value is not null;
        protected override string ToLabel(ScheduleField field)
        {
            return $"[{field.FieldIndex}] {field.ColumnHeading}";
        }
    }
}
