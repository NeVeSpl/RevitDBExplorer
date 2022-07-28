// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class BoolType : Base.ValueType<bool>
    {
        protected override bool CanBeSnoooped(bool boolean) => false;
        protected override string ToLabel(bool boolean) => boolean.ToString();        
    }
}