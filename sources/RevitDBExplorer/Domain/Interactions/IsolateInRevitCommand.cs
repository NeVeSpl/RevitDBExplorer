using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Interactions
{
    internal class IsolateInRevitCommand : BaseCommand
    {
        public static readonly IsolateInRevitCommand Instance = new IsolateInRevitCommand();

        public override bool CanExecute(object parameter)
        {
            if (parameter is TreeItem treeViewItem)
            {
                var elements = treeViewItem.GetAllSnoopableObjects().Where(x => x.Object is Element);

                if (elements.Any())
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
                var elementIds = treeViewItem.GetAllSnoopableObjects().Select(x => x.Object).OfType<Element>().Select(x => x.Id).ToList();
                if (elementIds.Any())
                {
                    ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync(x =>
                    {
                        var view = x.ActiveUIDocument?.Document?.ActiveView;
                        if (view is null)
                        {
                            return;
                        }
                        if (view.IsTemporaryHideIsolateActive())
                        {
                            view.DisableTemporaryViewMode(Autodesk.Revit.DB.TemporaryViewMode.TemporaryHideIsolate);
                        }

                        view.IsolateElementsTemporary(elementIds);
                    }, null, nameof(IsolateInRevitCommand));
                }
            }
        }
    }
}