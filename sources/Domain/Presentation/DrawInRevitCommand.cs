using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using RevitDBExplorer.UIComponents.Tree;
using RevitDBExplorer.UIComponents.Tree.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Presentation
{
    internal class DrawInRevitCommand : BaseCommand
    {
        public static readonly DrawInRevitCommand Instance = new DrawInRevitCommand();

        public override bool CanExecute(object parameter)
        {
            if (parameter is SnoopableObjectTreeItem treeViewItem)
            {
                if (treeViewItem.Object?.Object is GeometryObject)
                {
                    return true;
                }
                if (treeViewItem.Object?.Object is BoundingBoxXYZ)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Execute(object parameter)
        {
            if (parameter is SnoopableObjectTreeItem treeViewItem)
            {                
                ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync(x =>
                {
                    if (treeViewItem.Object?.Object is GeometryObject geometryObject)
                    {
                        Draw(treeViewItem.Object.Context.Document, geometryObject);
                    }
                    if (treeViewItem.Object?.Object is BoundingBoxXYZ boundingBoxXYZ)
                    {
                        Draw(treeViewItem.Object.Context.Document, boundingBoxXYZ);
                    }
                }, null, nameof(DrawInRevitCommand));      
            }
        }


        private static void Draw(Document document, GeometryObject geometryObject)
        {
            if (geometryObject is GeometryElement geometryElement)
            {
                DrawGeometryElement(document, geometryElement);
            }
            if (geometryObject is GeometryInstance geometryInstance)
            {
                DrawGeometryInstance(document, geometryInstance);
            }
            if (geometryObject is Solid solid)
            {
                DrawSolid(document, solid);
            }
            if (geometryObject is Face face)
            {
                DrawFace(document, face);
            }
            if (geometryObject is Edge edge)
            {
                DrawEdge(document, edge);
            }
            if (geometryObject is Curve curve)
            {
                DrawCurve(document, curve);
            }
        }
        private static void Draw(Document document, BoundingBoxXYZ bb)
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
          
            foreach (var edge in edges)
            {
                DrawCurve(document, edge);
            }
        }


        private static void DrawGeometryElement(Document document, GeometryElement geoElement)
        {
            foreach (var geometryObject in geoElement)
            {
                Draw(document, geometryObject);
            }
        }
        private static void DrawGeometryInstance(Document document, GeometryInstance geometryInstance)
        {
            DrawGeometryElement(document, geometryInstance.GetInstanceGeometry());
        }
        private static void DrawSolid(Document document, Solid solid)
        {
            foreach (Face  face in solid.Faces)
            {
                DrawFace(document, face);
            }
        }
        private static void DrawFace(Document document, Face face)
        {
            foreach (EdgeArray loop in face.EdgeLoops)
            {
                foreach (Edge edge in loop)
                {
                    DrawEdge(document, edge);
                }
            }
        }
        private static void DrawEdge(Document document, Edge edge)
        {
            DrawCurve(document, edge.AsCurve());
        }
        private static void DrawCurve(Document document, Curve curve)
        {
            IList<XYZ> points = new List<XYZ>();             

            if (curve is Line)
            {
                points.Add(curve.GetEndPoint(0));
                points.Add(curve.GetEndPoint(1));
            }
            else
            {
                points = curve.Tessellate();
            }

            Plane plane = null;
            XYZ p1 = points.First();
            XYZ p2 = points.Last();

            if (points.Count == 2)
            {
                plane = Plane.CreateByNormalAndOrigin(p1.CrossProduct(p2), p1);
            }
            if (points.Count > 2)
            {
                XYZ p3 = points[points.Count / 2];
                plane = Plane.CreateByThreePoints(p1, p2, p3);
            }

            var sketchPlane = SketchPlane.Create(document, plane);
            var modelCurve = document.Create.NewModelCurve(curve, sketchPlane);
        }
    }
}
