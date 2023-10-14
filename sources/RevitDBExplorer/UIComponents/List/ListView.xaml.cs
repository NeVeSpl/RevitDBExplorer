using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List
{
    public partial class ListView : UserControl
    {  
        public ListView()
        {
            InitializeComponent();           
        }

        private void ContextMenuForGroup_Copy_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var menu = menuItem.Parent as ContextMenu;
            var item = menu.PlacementTarget as GroupItem;
            var content = item.Content as CollectionViewGroup;

            Clipboard.SetDataObject(content?.Name);
        }
           
        

        private Point? _initialMousePosition  = null;
        private string _textToTransfer = null;
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {           
            _initialMousePosition = e.GetPosition(this);
            _textToTransfer = null;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                object dataContext;
                if (Mouse.DirectlyOver is FrameworkContentElement frameworkContentElement)
                {
                    dataContext = frameworkContentElement.DataContext;
                }
                if (Mouse.DirectlyOver is FrameworkElement frameworkElement)
                {
                    dataContext = frameworkElement.DataContext;
                }

                if (Mouse.DirectlyOver is TextBlock textBlock)
                {
                    _textToTransfer = textBlock.Text;
                }
                if (Mouse.DirectlyOver is Run run)
                {
                    _textToTransfer = run.Text;
                }
            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed && _initialMousePosition.HasValue)
            {
                var movedDistance = (_initialMousePosition.Value - e.GetPosition(this)).Length;
                if (movedDistance < 7) return;

                string textValue = _textToTransfer;

                if (string.IsNullOrWhiteSpace(textValue)) return;

                var bracketIndex = textValue.IndexOf('(');
                if (bracketIndex > 0)
                {
                    textValue = textValue.Substring(0, bracketIndex).Trim();
                }

                if (string.IsNullOrWhiteSpace(textValue)) return;

                DataObject data = new DataObject();
                data.SetData(DataFormats.StringFormat, textValue);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
                _initialMousePosition = null;

                e.Handled = false;
            }
        }
    }
}