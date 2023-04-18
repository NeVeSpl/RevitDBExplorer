using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System
{
    internal static class DoubleExtensions
    {
        public static string ToLengthDisplayString(this double value, Units units)
        {
            if (units != null)
                return UnitFormatUtils.Format(units, SpecTypeId.Length, value, false,
                                       new FormatValueOptions { AppendUnitSymbol = true });
            else
                return string.Empty;
        }
    }
}
