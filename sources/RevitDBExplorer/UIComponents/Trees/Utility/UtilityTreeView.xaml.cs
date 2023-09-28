using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RevitDBExplorer.Domain.DataModel;

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
            if (e.Data.GetDataPresent("RDC_Input") == false)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

      

        private void Grid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent("RDC_Input"))
            {
                var input = e.Data.GetData("RDC_Input") as SnoopableObject;

                if (input != null)
                {
                    var context = this.DataContext as UtilityTreeViewModel;
                    context.AddObject(input);
                }                
            }
            e.Handled = true;
        }
    }
}