using System.Windows;
using System.Windows.Controls;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Trees.Base.Items;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Utility
{
    public partial class UtilityTreeView : UserControl
    {
        public UtilityTreeView()
        {           
            InitializeComponent();
        }


        private void Grid_DragOver(object sender, System.Windows.DragEventArgs e)
        { 
            if ((e.Data.GetDataPresent("RDC_Input") == false) && (e.Data.GetDataPresent("RDC_Move") == false))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            if (e.Data.GetDataPresent("RDC_Move") == true)
            {
                if (e.Source is Button btn)
                {
                    return;
                }               
                if (e.OriginalSource is FrameworkElement fe && fe.DataContext is not SnoopableObjectTreeItem)
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }               
            }
        }      

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            base.OnDrop(e);
            var context = this.DataContext as UtilityTreeViewModel;

            if (e.Data.GetDataPresent("RDC_Input"))
            {
                var input = e.Data.GetData("RDC_Input") as SnoopableObject;

                if (input != null)
                {                    
                    context.AddObject(input);
                }                
            }

            if (e.Data.GetDataPresent("RDC_Move"))
            {
                var input = e.Data.GetData("RDC_Move") as SnoopableObjectTreeItem;
                if (e.OriginalSource is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.DataContext is SnoopableObjectTreeItem target)
                    {
                        context.MoveItem(input, target);
                    }
                }
                if (e.Source is Button button && button.Command is not null) 
                {
                    button.Command.Execute(input);
                }
            }

            e.Handled = true;
        }
    }
}