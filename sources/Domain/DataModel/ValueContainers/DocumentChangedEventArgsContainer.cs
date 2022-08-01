using System;
using Autodesk.Revit.DB.Events;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class DocumentChangedEventArgsContainer : Base.ValueContainer<DocumentChangedEventArgs>
    {
        protected override bool CanBeSnoooped(DocumentChangedEventArgs args) => true;
        protected override string ToLabel(DocumentChangedEventArgs args)
        {
            var transactionName = String.Join(", ", args.GetTransactionNames());

            return $"DocumentChanged: {transactionName}";
        }
    }
}