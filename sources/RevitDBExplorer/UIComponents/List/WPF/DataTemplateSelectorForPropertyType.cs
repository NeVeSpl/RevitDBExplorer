using System.Windows;
using System.Windows.Controls;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List.WPF
{
    internal class DataTemplateSelectorForPropertyType : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            var snoopableItem = item as SnoopableItem;

            if (snoopableItem?.ValueViewModel is DefaultPresenter { ValueContainer: not null } presenter)
            {
                var type = presenter.ValueContainer.TypeHandlerType;
                if (type != typeof(object))
                {
                    var key = new DataTemplateKey(type);
                    var dataTemplate = (DataTemplate)element.TryFindResource(key);
                    if (dataTemplate != null)
                    {
                        return dataTemplate;
                    }
                }
            }
            if (snoopableItem?.ValueViewModel is not null)
            {
                var key = new DataTemplateKey(snoopableItem.ValueViewModel.GetType());
                var dataTemplate = (DataTemplate)element.TryFindResource(key);
                return dataTemplate;
            }    

            return base.SelectTemplate(item, container);
        }
    }
}