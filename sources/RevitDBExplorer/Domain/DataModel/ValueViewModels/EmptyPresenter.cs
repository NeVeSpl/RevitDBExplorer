using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueViewModels
{
    internal class EmptyPresenter : IValuePresenter
    {
        public static readonly EmptyPresenter Instance = new EmptyPresenter();


        public string Label => string.Empty;
    }
}