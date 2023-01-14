using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class BoolHandler : TypeHandler<bool>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, bool boolean) => false;
        protected override string ToLabel(SnoopableContext context, bool boolean) => boolean.ToString();        
    }
}