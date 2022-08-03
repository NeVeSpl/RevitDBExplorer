using System.Windows;
using System.Windows.Controls;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

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

                if (snoopableMember.HasValue)
                { 
                    var key = new System.Windows.DataTemplateKey(snoopableMember.ValueContainer.GetType());
                    var dataTemplate = (DataTemplate)element.TryFindResource(key);
                    return dataTemplate;
                }

                if (snoopableMember.IsWritable)
                {
                    var key = new System.Windows.DataTemplateKey(typeof(IMemberAccessorWithWrite));
                    var dataTemplate = (DataTemplate)element.TryFindResource(key);
                    return dataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}