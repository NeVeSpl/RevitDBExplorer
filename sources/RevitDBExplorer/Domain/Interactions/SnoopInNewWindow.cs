using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Interactions
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
                    var snoopableObject = new SnoopableObject(treeViewItem.Object.Context.Document, treeViewItem.Object.Object);
                    var window = new MainWindow(new(new[] { snoopableObject }) { Info = new InfoAboutSource(snoopableObject.Name) }, Application.RevitWindowHandle);                   
                    window.Show();
                }
            }
        }
    }
}