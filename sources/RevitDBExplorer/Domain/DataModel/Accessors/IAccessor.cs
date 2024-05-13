using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.Accessors
{
    internal interface IAccessor
    {
        IValueViewModel CreatePresenter(SnoopableContext context, object @object);
        string UniqueId { get; set; }

        Invocation DefaultInvocation { get; }      
    }
}