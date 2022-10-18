// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Extensions.System;

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class DoubleContainer : Base.ValueContainer<double>, IHaveToolTip
    {
        protected override bool CanBeSnoooped(double doubleValue) => false;
        protected override string ToLabel(double doubleValue)
        {
            return doubleValue.ToString();
        }

        public string ToolTip
        {
            get => Value.ToLengthDisplayString(Units);
        }
    }
}