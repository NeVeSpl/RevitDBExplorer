using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

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
                    var parameter = uiElement.DataContext;

                    if (e.Source is ContentControl contentControl)
                    {
                        parameter = contentControl.Content;
                    }

                    cmd?.Execute(parameter);
                }
            }
        }
    }
}