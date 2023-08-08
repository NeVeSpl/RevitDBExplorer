using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueViewModels
{
    internal class DefaultPresenter : BaseViewModel, IValuePresenter, ICanRead, ICanSnoop
    {
        private readonly IMemberAccessorWithSnoop memberAccessor;
        private IValueContainer valueContainer;
        private string label;


        public IValueContainer ValueContainer
        {
            get
            {
                return valueContainer;
            }
            set
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


        public DefaultPresenter(IMemberAccessorWithSnoop memberAccessor)
        {
            this.memberAccessor = memberAccessor;
        }

        public void Read(SnoopableContext context, object @object)
        {
            var result = memberAccessor.Read(context, @object);          
            ValueContainer = result.State;
            Label = result.Label;
            CanBeSnooped = result.CanBeSnooped;
        }

        public IEnumerable<SnoopableObject> Snoop(SnoopableContext context, object @object)
        {
            if (memberAccessor is IMemberAccessorWithSnoop snooper)
            {
                return snooper.Snoop(context, @object, valueContainer);
            }
            return Enumerable.Empty<SnoopableObject>();
        }
    }
}