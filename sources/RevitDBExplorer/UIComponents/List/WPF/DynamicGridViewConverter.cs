using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RevitDBExplorer.WPF.Converters;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List.WPF
{
    internal class DynamicGridViewConverter : Freezable, IValueConverter
    {
        public FrameworkElement FrameworkElement
        {
            get { return (FrameworkElement)GetValue(FrameworkElementProperty); }
            set { SetValue(FrameworkElementProperty, value); }
        }


        public static readonly DependencyProperty FrameworkElementProperty = DependencyProperty.Register("FrameworkElement", typeof(FrameworkElement), typeof(DynamicGridViewConverter), new PropertyMetadata(null));


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var columns = value as IEnumerable<DynamicGridViewColumn>;
          
            if ((columns != null) && (FrameworkElement != null))
            {
                var gridView = new GridView();
                foreach (var column in columns)
                {
                    var gridColumn = new GridViewColumn { Header = column.Header };

                    var b = new Binding("ActualWidth");
                    b.Source = FrameworkElement;                   
                    b.Converter = (IValueConverter)FrameworkElement.TryFindResource("PercentageOfValueConverter");
                    b.ConverterParameter = column.Width;
                    BindingOperations.SetBinding(gridColumn, GridViewColumn.WidthProperty, b);

                    if (!string.IsNullOrEmpty(column.CellTemplate))
                    {
                        var dataTemplate = (DataTemplate)FrameworkElement.TryFindResource(column.CellTemplate);
                        if (dataTemplate != null)
                        {
                            gridColumn.CellTemplate = dataTemplate;
                        }
                    }
                    if (!string.IsNullOrEmpty(column.Binding))
                    {
                        var dataTemplate = new DataTemplate();
                        var elementFactory = new FrameworkElementFactory(typeof(ContentControl));
                        elementFactory.SetBinding(ContentControl.ContentProperty, new Binding(column.Binding));
                        dataTemplate.VisualTree = elementFactory;
                        gridColumn.CellTemplate = dataTemplate;                        
                    }

                    gridView.Columns.Add(gridColumn);
                }
                return gridView;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        protected override Freezable CreateInstanceCore()
        {
            return new ToResourceWithKeyConverter();
        }
    }

    internal class DynamicGridViewColumn
    {
        public string Header { get; set; }
        public string CellTemplate { get; set; }
        public string Width { get; set; }
        public string Binding { get; set; }


        public DynamicGridViewColumn(string header, int width)
        {
            this.Header = header;           
            this.Width = width.ToString();
        }
    }
}