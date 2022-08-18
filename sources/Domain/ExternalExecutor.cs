using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class ExternalExecutor
    {
        private static ExternalEvent externalEvent;

        public static void CreateExternalEvent()
        {
            externalEvent = ExternalEvent.Create(new ExternalEventHandler());
        }
        public static Task<T> ExecuteInRevitContextAsync<T>(Func<UIApplication, T> command)
        {
            var request = new Request<T>(command);
            ExternalEventHandler.Queue.Enqueue(request);
            externalEvent.Raise();
            return request.Task;
        }
        public static Task ExecuteInRevitContextAsync(Action<UIApplication> command)
        {           
            return ExecuteInRevitContextAsync<bool>(x => { command.Invoke(x); return true; });
        }


        private class Request<T> : IRequest
        {
            private readonly Func<UIApplication, T> command;
            private readonly TaskCompletionSource<T> completionSource = new();
            public Task<T> Task => completionSource.Task;


            public Request(Func<UIApplication, T> command)
            {
                this.command = command;
            }


            void IRequest.ExecuteCommand(UIApplication app)
            {
                try
                {
                    var result = command.Invoke(app);
                    completionSource.SetResult(result);
                }
                catch (Exception e)
                {
                    completionSource.SetException(e);
                }
            }
        }

        private interface IRequest
        {
            void ExecuteCommand(UIApplication app);
        }

        private class ExternalEventHandler : IExternalEventHandler
        {
            public static readonly ConcurrentQueue<IRequest> Queue = new();

            public void Execute(UIApplication app)
            {
                while (Queue.TryDequeue(out var request))
                {
                    request.ExecuteCommand(app);
                }
            }

            public string GetName()
            {
                return "RevitDBExplorer::ExternalExecutor::ExternalEventHandler";
            }
        }
    }

    internal static class ExternalExecutorExt
    {
        public static Task ExecuteInRevitContextInsideTransactionAsync(Action<UIApplication> command, Document document, string transactionName)
        {
            return ExternalExecutor.ExecuteInRevitContextAsync((x) =>
            {
                string fullTransactionName = $"RDBE::{transactionName}";
                Transaction transaction = null;
                try
                {
                    var revitDocument = document ?? x.ActiveUIDocument.Document;
                    transaction = revitDocument.IsModifiable == false ? new Transaction(revitDocument, fullTransactionName) : null;
                    transaction?.Start();
                    command?.Invoke(x);
                    transaction?.Commit();
                    
                }
                catch (Exception ex)
                {
                    transaction?.RollBack();
                    ex.ShowErrorMsg($"ExternalExecutorExt : {fullTransactionName}");
                }
                finally
                {
                    transaction?.Dispose();
                }
            });
        }
    }
}