using System;
using System.Globalization;
using System.Windows.Data;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ValueContainers;

namespace RevitDBExplorer.WPF.Converters
{
    internal class FeetToMetersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var snoopableMember = value as SnoopableMember;

            if (snoopableMember == null) return null;

            if (snoopableMember.ValueContainer is DoubleContainer container)
            {
                double result = container.Value * 0.3048;
                return $"{result} [m]";
            }
            return snoopableMember.ValueContainer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
