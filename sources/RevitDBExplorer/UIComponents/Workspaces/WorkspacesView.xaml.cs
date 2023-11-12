using System.Windows.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Workspaces
{
    public partial class WorkspacesView : UserControl
    {
        public WorkspacesView()
        {
            InitializeComponent();
        }

        private void Grid_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (this.DataContext is WorkspacesViewModel workspacesViewModel) 
            {
                workspacesViewModel.Width = e.NewSize.Width;
            }
        }
    }
}
