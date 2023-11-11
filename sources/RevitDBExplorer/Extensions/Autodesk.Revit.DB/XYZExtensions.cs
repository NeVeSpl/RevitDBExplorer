using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.DB
{
    public static class XYZExtensions
    {
        public static bool IsParallelTo(this XYZ first, XYZ second)
        {
            return first.CrossProduct(second).IsZeroLength();
        }

        public static bool IsPerpendicularTo(this XYZ first, XYZ second)
        {
            return Math.Abs(first.DotProduct(second)) < 1e-6;
        }

        public static bool IsCodirectionalTo(this XYZ first, XYZ second)
        {
            var dotProduct = first.Normalize().DotProduct(second.Normalize());

            return Math.Abs(dotProduct - 1.0) < 1e-6;
        }

        public static XYZ Min(this XYZ first, XYZ second) 
        {
            return new XYZ(Math.Min(first.X, second.X), Math.Min(first.Y, second.Y), Math.Min(first.Z, second.Z));
        }

        public static XYZ Max(this XYZ first, XYZ second)
        {
            return new XYZ(Math.Max(first.X, second.X), Math.Max(first.Y, second.Y), Math.Max(first.Z, second.Z));
        }
    }
}
