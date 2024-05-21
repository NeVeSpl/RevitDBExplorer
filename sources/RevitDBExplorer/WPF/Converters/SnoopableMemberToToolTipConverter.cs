using System;
using System.Globalization;
using System.Windows.Data;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;

namespace RevitDBExplorer.WPF.Converters
{
    internal class SnoopableMemberToToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DefaultPresenter presenter)
            {
                if (!string.IsNullOrEmpty(presenter.ToolTip))
                {
                    return presenter.ToolTip;
                }
                return presenter.Label; 
            }

            if (value is ErrorPresenter errorPresenter)
            {
                return errorPresenter.Label;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
