using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class ParameterType : Base.ValueType<Parameter>
    {
        protected override bool CanBeSnoooped(Parameter parameter) => false;
        protected override string ToLabel(Parameter parameter)
        {
            return parameter?.Definition?.Name;
        }
    }
}