using System.Collections.Generic;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueViewModels
{
    internal class DefaultPresenter : BaseViewModel, IValuePresenter, ICanRead, ICanSnoop
    {
        private readonly IAccessorForDefaultPresenter accessor;
        private IValueContainer valueContainer;
        private string label;


        public IValueContainer ValueContainer
        {
            get
            {
                return valueContainer;
            }
            private set
            {
                valueContainer = value;
                OnPropertyChanged();
            }
        }
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
                OnPropertyChanged();
            }
        }
        public bool CanBeSnooped { get; private set; }
        public bool CanBeVisualized { get; private set; }
        public string ToolTip => ValueContainer?.ToolTip;


        public DefaultPresenter(IAccessorForDefaultPresenter accessor)
        {
            this.accessor = accessor;
        }


        public void Read(SnoopableContext context, object @object)
        {
            var result = accessor.Read(context, @object);          
            ValueContainer = result.State;
            Label = result.Label;
            CanBeSnooped = result.CanBeSnooped;
            CanBeVisualized = result.CanBeVisualized;
        }

        public IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object)
        {          
            return accessor.Snoop(context, @object, valueContainer) ?? [];           
        }

        public IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, object @object)
        {            
            return accessor.GetVisualization(context, @object, valueContainer) ?? [];           
        }
    }
}