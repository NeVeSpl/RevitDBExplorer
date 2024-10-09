using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.DB
{
    public static class CurveArrayExtensions
    {
        public static IEnumerable<Curve> ToEnumerable(this CurveArray array)
        {
            foreach (Curve curve in array)
            {
                yield return curve;
            }
        }
    }
}