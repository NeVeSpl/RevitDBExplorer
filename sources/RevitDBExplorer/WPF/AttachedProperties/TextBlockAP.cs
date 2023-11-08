using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RevitDBExplorer.WPF.AttachedProperties
{
    internal class TextBlockAP
    {
        public static IEnumerable<Inline> GetBindableInlines(DependencyObject element)
        {
            return (IEnumerable<Inline>)element.GetValue(BindableInlinesProperty);
        }
        public static void SetBindableInlines(DependencyObject element, IEnumerable<Inline> value)
        {
            element.SetValue(BindableInlinesProperty, value);
        }

        public static readonly DependencyProperty BindableInlinesProperty = DependencyProperty.RegisterAttached("BindableInlines", typeof(IEnumerable<Inline>), typeof(TextBlockAP), new PropertyMetadata(null, BindableInlinesPropertyChanged));


        private static void BindableInlinesPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = element as TextBlock;

            if ((textBlock != null))
            {
                textBlock.Inlines.Clear();
                if ((e.NewValue is IEnumerable<Inline> inlines))
                {
                    textBlock.Inlines.AddRange(inlines);
                }
            }
        }
    }
}