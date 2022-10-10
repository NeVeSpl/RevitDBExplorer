using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitDBExplorer.Domain.DataModel.ValueContainers.Base
{
    internal static class ToDisplayUnitExtensions
    {
        public static string ToLengthDisplayString(this double value, Units units) =>
            UnitFormatUtils.Format(units, SpecTypeId.Length, value, false, new FormatValueOptions { AppendUnitSymbol = true});
    }
}
