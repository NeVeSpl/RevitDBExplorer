using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal class EventListener
    {
        public static event EventHandler<SelectionChangedEventArgs> SelectionChanged; 



        public static void Register(UIControlledApplication application)
        {
            application.SelectionChanged += Application_SelectionChanged;
        }


        


        private static void Application_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }
    }
}