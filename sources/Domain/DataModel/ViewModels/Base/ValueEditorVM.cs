using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ViewModels.Base
{
    internal class ValueEditorVM : BaseViewModel, IValueEditor
    {
        private RelayCommand writeCommand;

        public RelayCommand WriteCommand
        {
            get
            {
                return writeCommand;
            }
            set
            {
                writeCommand = value;
                OnPropertyChanged();
            }
        }
    }
}
