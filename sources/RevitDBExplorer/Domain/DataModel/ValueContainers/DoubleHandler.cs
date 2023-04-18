using System;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class DoubleHandler : TypeHandler<double>, IHaveToolTip<double>
    {       
        protected override bool CanBeSnoooped(SnoopableContext context, double doubleValue) => false;
        protected override string ToLabel(SnoopableContext context, double doubleValue)
        {
            return doubleValue.ToString();
        }
        public string GetToolTip(SnoopableContext context, double value)
        {
            var units = context.Document?.GetUnits();
            if (units != null)
            {
                return value.ToLengthDisplayString(units);
            }
            return value.ToString();
        }
       
    }
}