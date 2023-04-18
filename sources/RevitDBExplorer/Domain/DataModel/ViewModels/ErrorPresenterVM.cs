using RevitDBExplorer.Domain.DataModel.ViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ViewModels
{
    internal class ErrorPresenterVM : ValuePresenterVM
    {
        public ErrorPresenterVM(string error)
        {
            label = error;
        }
    }
}