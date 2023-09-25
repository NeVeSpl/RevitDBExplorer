using System;
using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDBExplorer.API.Demo
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RunDemoCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            // 

            var selectedIds = commandData.Application.ActiveUIDocument.Selection.GetElementIds();
            var document = commandData.Application.ActiveUIDocument.Document;

            FilteredElementCollector collector = null;

            if (selectedIds.Any())
            {
                collector = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(selectedIds));
            }
            else
            {
                collector = new FilteredElementCollector(document, document.ActiveView.Id);
            }

            var elements = collector.ToElements();

            //
            try
            {
                var controller = RevitDBExplorer.CreateController();
                controller.Snoop(document, elements);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "RevitDBExplorer.API.Demo", MessageBoxButton.OK, MessageBoxImage.Error);
                return Result.Failed;
            }

            //

            return Result.Succeeded;
        }
    }
}