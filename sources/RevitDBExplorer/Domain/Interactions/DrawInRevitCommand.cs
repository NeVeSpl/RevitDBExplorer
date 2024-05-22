using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Interactions
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
                        Draw(treeViewItem.Object.Context.Document, geometryObject.StreamCurves());
                    }
                    if (treeViewItem.Object?.Object is BoundingBoxXYZ boundingBoxXYZ)
                    {
                        Draw(treeViewItem.Object.Context.Document, boundingBoxXYZ.GetEdges());
                    }
                }, null, nameof(DrawInRevitCommand));      
            }
        }


        private static void Draw(Document document, IEnumerable<Curve> curves)
        {            
            foreach (var curve in curves)
            {
                DrawCurve(document, curve);
            }
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