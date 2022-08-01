using System;
using Autodesk.Revit.DB.Events;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class FailuresProcessingEventArgsContainer : Base.ValueContainer<FailuresProcessingEventArgs>
    {
        protected override bool CanBeSnoooped(FailuresProcessingEventArgs args) => true;
        protected override string ToLabel(FailuresProcessingEventArgs args)
        {
            var transactionName = args.GetFailuresAccessor()?.GetTransactionName();


            return $"FailuresProcessing: {transactionName}";
        }
    }
}
