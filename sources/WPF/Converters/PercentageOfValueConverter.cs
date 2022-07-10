using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitDBExplorer.WPF.Converters
{
    internal class PercentageOfValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double hostControlWidth = Math.Max((double.Parse(value.ToString())) - 30, 10);
            double percentage = (int.Parse(parameter.ToString())) / 100.0;
            double result = hostControlWidth * percentage;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}