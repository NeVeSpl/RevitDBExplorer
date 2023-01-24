using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.DB
{
    internal static class BoundingBoxXYZExtensions
    {
        public static IList<Line> GetEdges(this BoundingBoxXYZ bb)
        {
            var min = bb.Transform.OfPoint(bb.Min);
            var max = bb.Transform.OfPoint(bb.Max);

            XYZ pt0 = new XYZ(min.X, min.Y, min.Z);
            XYZ pt1 = new XYZ(max.X, min.Y, min.Z);
            XYZ pt2 = new XYZ(min.X, max.Y, min.Z);
            XYZ pt3 = new XYZ(min.X, min.Y, max.Z);
            XYZ pt4 = new XYZ(max.X, max.Y, max.Z);
            XYZ pt5 = new XYZ(min.X, max.Y, max.Z);
            XYZ pt6 = new XYZ(max.X, min.Y, max.Z);
            XYZ pt7 = new XYZ(max.X, max.Y, min.Z);

            var edges = new List<Line>()
            {
                Line.CreateBound(pt0, pt1),
                Line.CreateBound(pt0, pt2),
                Line.CreateBound(pt0, pt3),

                Line.CreateBound(pt1, pt6),
                Line.CreateBound(pt1, pt7),

                Line.CreateBound(pt2, pt5),
                Line.CreateBound(pt2, pt7),

                Line.CreateBound(pt3, pt5),
                Line.CreateBound(pt3, pt6),

                Line.CreateBound(pt4, pt5),
                Line.CreateBound(pt4, pt6),
                Line.CreateBound(pt4, pt7),
            };

            return edges;
        }     
    }
}