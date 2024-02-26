using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.WPF.AttachedProperties
{
    internal class HyperlinkAP
    {
        public static readonly DependencyProperty OpenOnClickProperty = DependencyProperty.RegisterAttached("OpenOnClick", typeof(bool), typeof(ButtonAP), new PropertyMetadata(false, OpenOnClickkPropertyChanged));


        public static bool GetOpenOnClick(DependencyObject element)
        {
            return (bool)element.GetValue(OpenOnClickProperty);
        }
        public static void SetOpenOnClick(DependencyObject element, bool value)
        {
            element.SetValue(OpenOnClickProperty, value);
        }


        private static void OpenOnClickkPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is Hyperlink hyperlink)
            {
                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            }
        }

        private static void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}