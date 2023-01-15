using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ViewModels.Base
{
    internal class ValuePresenterVM : BaseViewModel, IValuePresenter
    {
        protected string label;

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
    }
}
