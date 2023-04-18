using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ViewModels
{
    internal class DefaultPresenterVM : ValuePresenterVM
    {
        private IValueContainer valueContainer;
       


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