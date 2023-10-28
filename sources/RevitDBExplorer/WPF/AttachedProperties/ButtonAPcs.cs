using System.Windows;
using System.Windows.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.WPF.AttachedProperties
{
    internal class ButtonAP
    {
        public static readonly DependencyProperty OpenSubMenuOnClickProperty = DependencyProperty.RegisterAttached("OpenSubMenuOnClick", typeof(bool), typeof(ButtonAP), new PropertyMetadata(false, OpenSubMenuOnClickPropertyChanged));


        public static bool GetOpenSubMenuOnClick(DependencyObject element)
        {
            return (bool)element.GetValue(OpenSubMenuOnClickProperty);
        }
        public static void SetOpenSubMenuOnClick(DependencyObject element, bool value)
        {
            element.SetValue(OpenSubMenuOnClickProperty, value);
        }


        private static void OpenSubMenuOnClickPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is Button button)
            {
                button.Click += Button_Click;
            }
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = ContextMenuService.GetContextMenu(sender as DependencyObject);
            if (contextMenu == null)
            {
                return;
            }
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.PlacementTarget = sender as UIElement;
            contextMenu.IsOpen = true;
        }
    }
}
