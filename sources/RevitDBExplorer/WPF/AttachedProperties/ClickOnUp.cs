using System.Windows;
using System.Windows.Input;

namespace RevitDBExplorer.WPF.AttachedProperties
{
    internal class ClickOnUp
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ClickOnUp), new PropertyMetadata(null, CommandPropertyChanged));


        public static ICommand GetCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }
        public static void SetCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

       


        private static void CommandPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is UIElement uiElement)
            {
                uiElement.PreviewMouseUp += UiElement_PreviewMouseUp;    
            }
        }

        private static void UiElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.Source is FrameworkElement uiElement)
                {
                    var cmd = GetCommand(uiElement);
                    cmd?.Execute(uiElement.DataContext);
                }
            }
        }
    }
}
