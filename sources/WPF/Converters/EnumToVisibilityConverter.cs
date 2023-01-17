using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitDBExplorer.WPF.Converters
{
    internal class EnumMatchToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value?.ToString();
            var strPar = parameter?.ToString(); 

            if (String.Equals(strValue, strPar, StringComparison.OrdinalIgnoreCase))
            {
                return Visibility.Visible;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
