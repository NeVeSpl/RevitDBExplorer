using System;
using Autodesk.Revit.DB.Events;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class DocumentChangedEventArgsHandler : TypeHandler<DocumentChangedEventArgs>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, DocumentChangedEventArgs args) => true;
        protected override string ToLabel(SnoopableContext context, DocumentChangedEventArgs args)
        {
            var transactionName = String.Join(", ", args.GetTransactionNames());

            return $"DocumentChanged: {transactionName}";
        }
    }
}