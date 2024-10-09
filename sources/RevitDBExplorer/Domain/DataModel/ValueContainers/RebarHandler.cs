using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitExplorer.Visualizations.DrawingVisuals;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class RebarHandler : TypeHandler<Rebar>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Rebar rebar) => rebar is not null;

        protected override string ToLabel(SnoopableContext context, Rebar rebar)
        {
            return Labeler.GetLabelForObjectWithId(rebar.Name, rebar.Id.Value());
        }

        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, Rebar rebar)
        {
            var freshElement = context.Document?.GetElement(rebar.Id) ?? rebar;
            yield return new SnoopableObject(context.Document, freshElement);
        }


        private readonly static Color ArrowColor = new Color(112, 48, 161);
        private readonly static Color StartColor = new Color(0, 255, 0);
        private readonly static Color EndColor = new Color(255, 0, 0);

        protected override bool CanBeVisualized(SnoopableContext context, Rebar rebar) => true;
        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, Rebar rebar)
        {
            if (rebar.IsRebarShapeDriven())
            {
                var refPoint = rebar.get_BoundingBox(null).CenterPoint();
               

                var rebarShapeDrivenAccessor = rebar.GetShapeDrivenAccessor();
                var normal = rebarShapeDrivenAccessor.Normal;

                yield return new VisualizationItem("Rebar", "GetShapeDrivenAccessor().Normal", new ArrowDrawingVisual(refPoint, normal, VisualizationItem.NormalColor));

                var curves = rebar.GetCenterlineCurves(false, true, true, MultiplanarOption.IncludeOnlyPlanarCurves, 0);
                var startPoint = curves.First().GetEndPoint(0);
                var endPoint = curves.Last().GetEndPoint(1);

                yield return new VisualizationItem("Rebar", "start", new CubeDrawingVisual(startPoint, VisualizationItem.StartColor));
                yield return new VisualizationItem("Rebar", "end", new CubeDrawingVisual(endPoint, VisualizationItem.EndColor));
            }
        }
    }
}