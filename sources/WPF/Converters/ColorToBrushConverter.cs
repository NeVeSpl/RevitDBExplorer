using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitDBExplorer.WPF.Converters
{
    internal class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return null;
                case System.Windows.Media.Color color:
                    return new SolidColorBrush(color);
                case Autodesk.Revit.DB.Color aColor:
                    if (aColor.IsValid)
                    {
                        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(aColor.Red, aColor.Green, aColor.Blue));
                    }
                    else
                    {
                        return Brushes.Transparent; 
                    }
            }
           
            throw new InvalidOperationException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}