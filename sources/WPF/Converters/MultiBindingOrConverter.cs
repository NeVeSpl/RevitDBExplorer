using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

// source : https://stackoverflow.com/questions/38396419/multidatatrigger-with-or-instead-of-and

namespace RevitDBExplorer.WPF.Converters
{
    public class MultiBindingOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(v => ReferenceEquals(v, DependencyProperty.UnsetValue)))
            {
                return DependencyProperty.UnsetValue;
            }
            return values.OfType<bool>().Any(b => b);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}