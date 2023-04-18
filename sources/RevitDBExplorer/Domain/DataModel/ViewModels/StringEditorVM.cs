using RevitDBExplorer.Domain.DataModel.ViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ViewModels
{
    internal class StringEditorVM : ValueEditorVM
    {
        private string value;


        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
    }
}