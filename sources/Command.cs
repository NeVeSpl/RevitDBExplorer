using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDBExplorer
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var window = new MainWindow();
            new WindowInteropHelper(window).Owner = commandData.Application.MainWindowHandle;
            window.Show();

            return Result.Succeeded;
        }
    }
}