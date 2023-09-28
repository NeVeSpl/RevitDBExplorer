using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using RevitDBExplorer.UIComponents.Trees.Base.Items;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Base
{
    public partial class BaseTreeView : UserControl
    {
        public BaseTreeView()
        {
            InitializeComponent();
        }


        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.DataContext is BaseTreeViewModel treeView)
            {
                if (e.NewValue is TreeItem treeViewItemVM)
                {
                    treeView.RaiseSelectedItemChanged(treeViewItemVM);
                }
            }
        }


        private Point _initialMousePosition;
        private object _clickedElementDataContext;
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            _initialMousePosition = e.GetPosition(this);
            _clickedElementDataContext = null;
            if (e.LeftButton == MouseButtonState.Pressed)
            {                
                if (Mouse.DirectlyOver is FrameworkContentElement frameworkContentElement)
                {
                    _clickedElementDataContext = frameworkContentElement.DataContext;
                }
                if (Mouse.DirectlyOver is FrameworkElement frameworkElement)
                {
                    _clickedElementDataContext = frameworkElement.DataContext;
                }
            }
            base.OnPreviewMouseDown(e);
        }
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);          

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var movedDistance = (_initialMousePosition - e.GetPosition(this)).Length;
                if (movedDistance < 7) return;

                if (_clickedElementDataContext is TreeItem treeItem)
                {
                    string text = "???";

                    if (_clickedElementDataContext is GroupTreeItem groupTreeItem)
                    {
                        text = groupTreeItem.Name;
                    }
                    if (_clickedElementDataContext is SnoopableObjectTreeItem snoopableObjectTreeItem)
                    {
                        text = snoopableObjectTreeItem?.Object?.Name;
                    }


                    DataObject data = new DataObject();
                    data.SetData("RDS_Inputs", BaseTreeViewModel.GetObjectsForTransfer(treeItem) ?? Enumerable.Empty<object>());
                    data.SetData(DataFormats.StringFormat, text ?? "");
                    if (_clickedElementDataContext is SnoopableObjectTreeItem snoopableObjectTreeItem2)
                    {
                        data.SetData("RDC_Input", snoopableObjectTreeItem2.Object);
                    }

                    DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
                    //e.Handled = true;
                }
            }
        }
    }
}