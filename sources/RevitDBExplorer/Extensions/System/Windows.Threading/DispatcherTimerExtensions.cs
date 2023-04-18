// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System.Windows.Threading
{
    internal static class DispatcherTimerExtensions
    {
        public static DispatcherTimer Debounce(this DispatcherTimer dispatcher, TimeSpan interval, Action action)
        {
            dispatcher?.Stop();
            dispatcher = null;
            dispatcher = new DispatcherTimer(interval,  DispatcherPriority.ApplicationIdle, (s, e) =>
            { 
                dispatcher?.Stop();              
                action.Invoke();
            }, Dispatcher.CurrentDispatcher);
            dispatcher?.Start();

            return dispatcher;
        }
    }
}