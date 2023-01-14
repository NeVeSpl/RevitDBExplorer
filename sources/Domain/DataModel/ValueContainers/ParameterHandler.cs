using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ParameterHandler : TypeHandler<Parameter>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Parameter parameter) => false;
        protected override string ToLabel(SnoopableContext context, Parameter parameter)
        {
            return parameter?.Definition?.Name;
        }
    }
}