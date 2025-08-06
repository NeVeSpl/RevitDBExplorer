using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal class EventListener
    {
        public static event Action<object> SelectionChanged; 



        public static void Register(UIControlledApplication application)
        {
#if R2023_MIN
            application.SelectionChanged += Application_SelectionChanged;
#endif
        }




#if R2023_MIN
        private static void Application_SelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            SelectionChanged?.Invoke(sender);
        }
#endif
    }
}