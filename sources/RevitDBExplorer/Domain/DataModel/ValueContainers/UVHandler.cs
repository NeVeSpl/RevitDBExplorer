using System;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class UVHandler : TypeHandler<UV>, IHaveToolTip<UV>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, UV uv) => false;
        protected override string ToLabel(SnoopableContext context, UV uv)
        {
            return $"({uv.U}, {uv.V})";
        }
        public string GetToolTip(SnoopableContext context, UV value)
        {            
            var units = context.Document?.GetUnits();
            if (units == null) return null;

            return $"{ToLabel(context, value)}\n({value.U.ToLengthDisplayString(units)}, {value.V.ToLengthDisplayString(units)})";
        }
    }
}