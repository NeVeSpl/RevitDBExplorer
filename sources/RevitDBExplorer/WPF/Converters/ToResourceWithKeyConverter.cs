using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RevitDBExplorer.WPF.Converters
{
    internal class ToResourceWithKeyConverter : Freezable,  IValueConverter
    {
        public FrameworkElement FrameworkElement
        {
            get { return (FrameworkElement)GetValue(FrameworkElementProperty); }
            set { SetValue(FrameworkElementProperty, value); }
        }

        
        public static readonly DependencyProperty FrameworkElementProperty =
            DependencyProperty.Register("FrameworkElement", typeof(FrameworkElement), typeof(ToResourceWithKeyConverter), new PropertyMetadata(null));


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (FrameworkElement is not null)
            {
                string name = $"{parameter}{value}";
                var resource = FrameworkElement.TryFindResource(name);
                return resource;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }




        protected override Freezable CreateInstanceCore()
        {
            return new ToResourceWithKeyConverter();
        }
    }
}
