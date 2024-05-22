using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.Interactions
{
    internal class DrawInRevitWithAVFCommand : BaseCommand
    {
        public static readonly DrawInRevitWithAVFCommand Instance = new DrawInRevitWithAVFCommand();

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
                }, null, nameof(DrawInRevitWithAVFCommand));
            }
        }


        private static void Draw(Document document, IEnumerable<Curve> curves)
        {
            var view = document.ActiveView;

            //
            var spatialFieldManager = SpatialFieldManager.GetSpatialFieldManager(view);
            if (spatialFieldManager == null) spatialFieldManager = SpatialFieldManager.CreateSpatialFieldManager(view, 1);

            //
            var displayStyleId = AnalysisDisplayStyle.FindByName(document, "RDBE_edge_style");
            var displayStyle = document.GetElement(displayStyleId) as AnalysisDisplayStyle;
            if (displayStyle == null)
            {
                displayStyle = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(
                  document,
                  "RDBE_edge_style",
                  new AnalysisDisplayDiagramSettings() { OutlineColor = new Color(255, 0, 0) },
                  new AnalysisDisplayColorSettings(),
                  new AnalysisDisplayLegendSettings() { ShowLegend = false });
            }

            //
            var schemaId = -1;

            foreach (var id in spatialFieldManager.GetRegisteredResults())
            {
                var schema = spatialFieldManager.GetResultSchema(id);
                if (schema.Name == "RDBE_edge_schema")
                {
                    schemaId = id;
                    break;
                }
            }

            if (schemaId == -1)
            {
                var schema = new AnalysisResultSchema("RDBE_edge_schema", "Description");
                schema.AnalysisDisplayStyleId = displayStyle.Id;
                schemaId = spatialFieldManager.RegisterResult(schema);
            }

            //
            foreach (var curve in curves)
            {
                DrawCurve(document, curve, spatialFieldManager, schemaId);
            }
        }
        
        private static void DrawCurve(Document document, Curve curve, SpatialFieldManager spatialFieldManager, int schemaId)
        {
            int idx = spatialFieldManager.AddSpatialFieldPrimitive(curve, Transform.Identity);

            var domain = new FieldDomainPointsByParameter(new[] { curve.GetEndParameter(0), curve.GetEndParameter(1) });
            var values = new FieldValues(new [] { new ValueAtPoint(new[] { 0.0 }), new ValueAtPoint(new[] { 0.0 }) });

            spatialFieldManager.UpdateSpatialFieldPrimitive(idx, domain, values, schemaId);
        }
    }
}