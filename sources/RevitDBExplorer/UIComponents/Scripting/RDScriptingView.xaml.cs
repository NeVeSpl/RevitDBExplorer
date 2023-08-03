using System.Windows;
using System.Windows.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Scripting
{
    public partial class RDScriptingView : UserControl
    {
        private RDScriptingVM scriptingVM;
       

        public RDScriptingView()
        {
            InitializeComponent();
            this.DataContextChanged += RDScriptingView_DataContextChanged;
        }


        private void RDScriptingView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is RDScriptingVM vm)
            {                
                scriptingVM = vm;
            }
        }


     


        private void TabControl_DragOver(object sender, DragEventArgs e)
        {

        }
        private void TabControl_Drop(object sender, DragEventArgs e)
        {
            
        }
    }
}