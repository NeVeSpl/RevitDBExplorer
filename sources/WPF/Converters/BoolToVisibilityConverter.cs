using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitDBExplorer.WPF.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility WhenFalse { get; set;  } = Visibility.Hidden;
        public Visibility WhenTrue{ get; set; } = Visibility.Visible;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;            

            if (boolValue)
            {
                return WhenTrue;
            }

            return WhenFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}