using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.WPF.Controls
{
    public partial class TextBoxWithPlaceholder : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata("Placeholder"));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }


        public TextBoxWithPlaceholder()
        {            
            InitializeComponent();
            cMainGrid.DataContext = this;
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
            }
        }
    }
}