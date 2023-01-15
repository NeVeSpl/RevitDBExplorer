using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ViewModels.Base
{
    internal interface IValueEditor : IValueViewModel
    {
        RelayCommand WriteCommand { get; }
    }    
}