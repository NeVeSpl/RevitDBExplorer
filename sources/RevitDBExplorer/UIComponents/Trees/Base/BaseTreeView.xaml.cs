using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RevitDBExplorer.UIComponents.Trees.Base.Items;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Base
{
    public partial class BaseTreeView : UserControl
    {
        public bool AllowDragOf_RDC_Input { get; set; } = true;
        public bool AllowDragOf_RDC_Move { get; set; } = false;


        public BaseTreeView()
        {
            InitializeComponent();
            cTreeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            cTreeView.AddHandler(TreeViewItem.MouseLeftButtonDownEvent, new MouseButtonEventHandler(TreeViewItem_MouseLeftButtonDown), true);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.DataContext is BaseTreeViewModel treeView)
            {
                if ((e.NewValue is TreeItem treeViewItemVM))
                {
                    treeView.RaiseSelectedItemChanged(treeViewItemVM);
                }
                if ((e.NewValue is null))
                {
                    treeView.RaiseSelectedItemChanged(null);
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
                        if (AllowDragOf_RDC_Input)
                        {
                            data.SetData("RDC_Input", snoopableObjectTreeItem2.Object);
                        }
                        if (AllowDragOf_RDC_Move)
                        {
                            data.SetData("RDC_Move", snoopableObjectTreeItem2);
                        }
                    }

                    var effect = DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
                    //e.Handled = true;
                }
            }
        }



        public void TreeViewItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = FindTreeViewItem(e.OriginalSource as DependencyObject);
            TreeView treeView = sender as TreeView;
            TreeItem treeItem = treeViewItem?.DataContext as TreeItem;

            if (treeViewItem == null || treeView == null || treeItem == null)
                return;

            if (this.DataContext is BaseTreeViewModel treeViewVM)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    treeViewVM.SelectSingleItem(treeItem, withCtrl: true);
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    //SelectMultipleItemsContinuously(treeView, treeViewItem);
                }
                else
                {
                    treeViewVM.SelectSingleItem(treeItem, withCtrl: false);
                }
            }
            e.Handled = true;
        }

        private static TreeViewItem FindTreeViewItem(DependencyObject dependencyObject)
        {          
            if (dependencyObject is System.Windows.Controls.Primitives.ButtonBase)
                return null;

            if (dependencyObject == null)
            {
                return null;
            }

            TreeViewItem treeViewItem = dependencyObject as TreeViewItem;
            if (treeViewItem != null)
            {
                return treeViewItem;
            }

            return FindTreeViewItem(VisualTreeHelper.GetParent(dependencyObject));
        }

    }
}