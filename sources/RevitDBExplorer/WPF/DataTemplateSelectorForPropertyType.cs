using System.Windows;
using System.Windows.Controls;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.ViewModels;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.WPF
{
    internal class DataTemplateSelectorForPropertyType : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SnoopableMember snoopableMember)
            {
                FrameworkElement element = container as FrameworkElement;

                if (snoopableMember.ValueViewModel != null)
                {
                    if (snoopableMember.ValueViewModel is DefaultPresenterVM { ValueContainer: not null } presenter)
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
                    {
                        var key = new DataTemplateKey(snoopableMember.ValueViewModel.GetType());
                        var dataTemplate = (DataTemplate)element.TryFindResource(key);
                        return dataTemplate;
                    }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}