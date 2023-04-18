using System.Windows.Interop;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Tree.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Presentation
{
    internal class SnoopInNewWindowCommand : BaseCommand
    {
        public static readonly SnoopInNewWindowCommand Instance = new SnoopInNewWindowCommand();

        public override bool CanExecute(object parameter)
        {
            if (parameter is SnoopableObjectTreeItem treeViewItem)
            {
                if (treeViewItem.Object != null)
                {
                    return true;
                }

            }
            return false;
        }

        public override void Execute(object parameter)
        {
            if (parameter is SnoopableObjectTreeItem treeViewItem)
            {
                if (treeViewItem.Object != null)
                {
                    var window = new MainWindow(new(new[] { new SnoopableObject(treeViewItem.Object.Context.Document, treeViewItem.Object.Object) }));
                    new WindowInteropHelper(window).Owner = Application.RevitWindowHandle;
                    window.Show();
                }
            }
        }
    }
}