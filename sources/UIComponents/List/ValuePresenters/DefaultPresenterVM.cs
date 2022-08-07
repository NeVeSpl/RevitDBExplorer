using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List.ValuePresenters
{
    internal class DefaultPresenterVM : BaseViewModel, IValuePresenter
    {
        public IValueContainer valueContainer;


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
    }
}