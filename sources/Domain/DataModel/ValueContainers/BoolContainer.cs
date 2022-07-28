// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class BoolContainer : Base.ValueContainer<bool>
    {
        protected override bool CanBeSnoooped(bool boolean) => false;
        protected override string ToLabel(bool boolean) => boolean.ToString();        
    }
}