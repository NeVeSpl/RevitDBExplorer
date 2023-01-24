using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitDBExplorer.WPF.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility WhenFalse { get; set;  } = Visibility.Hidden;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;            

            if (boolValue)
            {
                return Visibility.Visible;
            }

            return WhenFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}