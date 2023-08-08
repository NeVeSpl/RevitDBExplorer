using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueViewModels
{
    internal class ErrorPresenter : BaseViewModel, IValuePresenter
    {
        private string label;

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


        public ErrorPresenter(string error)
        {
            label = error;
        }
    }
}