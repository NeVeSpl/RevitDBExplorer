// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class DoubleType : Base.ValueType<double>
    {
        protected override bool CanBeSnoooped(double doubleValue) => false;
        protected override string ToLabel(double doubleValue)
        {
            return doubleValue.ToString();            
        }
    }
}