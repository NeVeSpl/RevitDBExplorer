using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
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
}