using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class FailureMessageHandler : TypeHandler<FailureMessage>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, FailureMessage value) => true;

        protected override string ToLabel(SnoopableContext context, FailureMessage value)
        {
            return $"FailureMessage: ({value.GetDescriptionText()})";
        }
    }
}
