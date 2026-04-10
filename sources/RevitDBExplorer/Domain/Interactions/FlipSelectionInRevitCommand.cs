using System;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Interactions
{
    internal class FlipSelectionInRevitCommand : BaseCommand
    {
        public static readonly FlipSelectionInRevitCommand Instance = new FlipSelectionInRevitCommand();


        public override bool CanExecute(object parameter)
        {
            if (parameter is TreeItem treeViewItem)
            {
                var isAvailable = treeViewItem.GetAllSnoopableObjects().All(x => IsSelectInRevitAvailable(x));

                if (isAvailable)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Execute(object parameter)
        {
            if (parameter is TreeItem treeViewItem)
            {
                var snoopableObjects = treeViewItem.GetAllSnoopableObjects().ToList();
                var elementIds = snoopableObjects.Select(x => x.Object).OfType<Element>().Select(x => x.Id).ToList();
                if (elementIds.Any())
                {
                    ExternalExecutor.ExecuteInRevitContextAsync(context =>
                    {
                        var activeUiDocument = context.ActiveUIDocument;
                        if (activeUiDocument == null) return;

                        var selection = activeUiDocument.Selection;
                        var currentSelection = selection.GetElementIds();

                        // Toggle each element id: remove if present, add if not
                        foreach (var elementId in elementIds)
                        {
                            if (currentSelection.Contains(elementId))
                            {
                                currentSelection.Remove(elementId);
                            }
                            else
                            {
                                currentSelection.Add(elementId);
                            }
                        }

                        selection.SetElementIds(currentSelection);
                    });
                }               
            }
            Application.RevitWindowHandle.BringWindowToFront();
        }


        public static bool IsSelectInRevitAvailable(SnoopableObject snoopableObject)
        {
            if (snoopableObject.Object is Element)
            {
                return true;
            }

            return false;
        }
    }
}