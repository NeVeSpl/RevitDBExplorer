// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class IntContainer : Base.ValueContainer<int>
    {
        protected override bool CanBeSnoooped(int intValue) => false;
        protected override string ToLabel(int intValue)
        {
            return intValue.ToString();
        }
    }
}