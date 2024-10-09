using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.DB
{
    internal static class BoundingBoxXYZExtensions
    {
        public static XYZ CenterPoint(this BoundingBoxXYZ bb)
        {
            if ((bb?.Max != null) && (bb?.Min != null))
            {
                return bb.Min.Add(0.5 * bb.Max.Subtract(bb.Min));
            }
            return XYZ.Zero;
        }


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


        public static Solid CreateSolidFromBoundingBox(this BoundingBoxXYZ bbox)
        {
            var pt0 = new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
            var pt1 = new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z);
            var pt2 = new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z);
            var pt3 = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);

            var edge0 = Line.CreateBound(pt0, pt1);
            var edge1 = Line.CreateBound(pt1, pt2);
            var edge2 = Line.CreateBound(pt2, pt3);
            var edge3 = Line.CreateBound(pt3, pt0);

            var edges = new Curve[]
            {
                edge0,
                edge1,
                edge2,
                edge3
            };

            double height = bbox.Max.Z - bbox.Min.Z;
           
            var loopList = new List<CurveLoop>() { CurveLoop.Create(edges) };          

            Solid preTransformBox = GeometryCreationUtilities.CreateExtrusionGeometry(loopList, XYZ.BasisZ, height);
            Solid transformBox = SolidUtils.CreateTransformed(preTransformBox, bbox.Transform);

            return transformBox;
        }
    }
}