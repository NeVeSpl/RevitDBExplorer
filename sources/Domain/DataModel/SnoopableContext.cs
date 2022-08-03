using System;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal record class SnoopableContext
    {
        public Document Document { get; init; }


        public Task Execute(Action<Document> command, string transactionName)
        {            
            return ExternalExecutor.ExecuteInRevitContextAsync((x) =>
            {
                Transaction transaction = null;
                try
                {
                    transaction = Document.IsModifiable == false ? new Transaction(Document, transactionName) : null;
                    transaction?.Start();
                    command(Document);
                    transaction?.Commit();
                }
                catch (Exception ex)
                {
                    transaction?.RollBack();
                    ex.ShowErrorMsg($"SnoopableContext.Execute : {transactionName}");
                }
                finally
                {
                    transaction?.Dispose();
                }
            });
        }
    }
}