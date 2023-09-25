using System.Collections.Generic;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.DB
{
    internal static class GeometryObjectExtensions
    {
        public static Reference GetReference(this GeometryObject geometryObject)
        {
            var reference = geometryObject switch
            {
                Face face => face.Reference,
                Edge edge => edge.Reference,
                Curve curve => curve.Reference,
                Point point => point.Reference,               
                _ => null,
            };

            return reference;
        }



        public static IEnumerable<Curve> StreamCurves(this GeometryObject geometryObject)
        {            
            if (geometryObject is GeometryElement geometryElement)
            {
                foreach (var geometryObject_ in geometryElement)
                {
                    var result = StreamCurves(geometryObject_);
                    foreach (var item in result) yield return item;
                }
            }
            if (geometryObject is GeometryInstance geometryInstance)
            {
                var result = StreamCurves(geometryInstance.GetInstanceGeometry());
                foreach (var item in result) yield return item;
            }
            if (geometryObject is Solid solid)
            {
                foreach (Face face_ in solid.Faces)
                {
                    var result = StreamCurves(face_);
                    foreach (var item in result) yield return item;
                }
            }
            if (geometryObject is Face face)
            {
                foreach (EdgeArray loop in face.EdgeLoops)
                {
                    foreach (Edge edge_ in loop)
                    {
                        var result = StreamCurves(edge_);
                        foreach (var item in result) yield return item;
                    }
                }
            }
            if (geometryObject is Edge edge)
            {
                var result = StreamCurves(edge.AsCurve());
                foreach (var item in result) yield return item;
            }
            if (geometryObject is Curve curve)
            {
                yield return curve;
            }
        }
    }
}