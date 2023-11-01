using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.Selectors;

namespace RevitDBExplorer
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        private static List<WeakReference> windows = new List<WeakReference>();


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var source = SelectorFactory.Create(Selector.CurrentSelection);
            source.ReadFromTheSource(commandData.Application);
            var window = new MainWindow(source, commandData.Application.MainWindowHandle);
            window.Show();

            windows.Add(new WeakReference(window));
            int numberOfWindowsInMemory = windows.Count(x => x.IsAlive);
            Debug.WriteLine($"==> number of windows in memory: {numberOfWindowsInMemory}");

            if (numberOfWindowsInMemory > 3)
            {
#if DEBUG
                GC.Collect();
                if (numberOfWindowsInMemory > 4)
                {
                    //throw new Exception("There is probably a memory leak.");
                }
#endif
            }

            return Result.Succeeded;
        }
    }
}