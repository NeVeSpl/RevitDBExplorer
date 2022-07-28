using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ParameterContainer : Base.ValueContainer<Parameter>
    {
        protected override bool CanBeSnoooped(Parameter parameter) => false;
        protected override string ToLabel(Parameter parameter)
        {
            return parameter?.Definition?.Name;
        }
    }
}