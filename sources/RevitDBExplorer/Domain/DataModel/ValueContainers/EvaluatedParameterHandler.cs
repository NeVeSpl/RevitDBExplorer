using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
#if R2024_MIN
    internal class EvaluatedParameterHandler : TypeHandler<EvaluatedParameter>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, EvaluatedParameter parameter) => false;
        protected override string ToLabel(SnoopableContext context, EvaluatedParameter parameter)
        {
            return parameter?.Definition?.Name;
        }
    }
#endif
}