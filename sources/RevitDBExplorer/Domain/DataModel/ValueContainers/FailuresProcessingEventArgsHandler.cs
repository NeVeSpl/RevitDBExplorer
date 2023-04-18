using System;
using Autodesk.Revit.DB.Events;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class FailuresProcessingEventArgsHandler : TypeHandler<FailuresProcessingEventArgs>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, FailuresProcessingEventArgs args) => true;
        protected override string ToLabel(SnoopableContext context, FailuresProcessingEventArgs args)
        {
            var transactionName = args.GetFailuresAccessor()?.GetTransactionName();


            return $"FailuresProcessing: {transactionName}";
        }
    }
}
