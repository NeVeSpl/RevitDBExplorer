using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.List.ValueEditors
{
    internal class IntegerEditorVM : BaseViewModel, IValueEditor
    {
        private int value;


        public int Value
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