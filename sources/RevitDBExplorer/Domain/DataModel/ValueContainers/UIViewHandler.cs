using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class UIViewHandler : TypeHandler<UIView>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, UIView value) => true;

        protected override string ToLabel(SnoopableContext context, UIView value) => Labeler.GetLabelForObjectWithId("GeometryElement", value.ViewId.Value());


        protected override bool CanBeVisualized(SnoopableContext context, UIView value) => true;


        private readonly static Color CrossColor = new Color(255, 0, 225);

        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, UIView uiView)
        {
            var view = context.Document.GetElement(uiView.ViewId) as View;
            IList<XYZ> corners = null;
            try
            {
                corners = uiView.GetZoomCorners();
            }
            catch
            {
                yield break;
            
            }

            yield return new VisualizationItem("UIView", "GetZoomCorners()[0]", new CrossDrawingVisual(corners[0], VisualizationItem.StartColor) { LineLength = 0.1 });
            yield return new VisualizationItem("UIView", "GetZoomCorners()[1]", new CrossDrawingVisual(corners[1], VisualizationItem.EndColor) { LineLength = 0.1 });

            var diagVector = corners[0] - corners[1];

            double height = Math.Abs(diagVector.DotProduct(view.UpDirection));
            double width = Math.Abs(diagVector.DotProduct(view.RightDirection));

            var p0 = corners[0];
            var p1 = corners[0] + width * view.RightDirection;
            var p2 = corners[1];
            var p3 = corners[0] + height * view.UpDirection;

            var l1 = Line.CreateBound(p0, p1);
            var l2 = Line.CreateBound(p1, p2);
            var l3 = Line.CreateBound(p2, p3);
            var l4 = Line.CreateBound(p3, p0);

            yield return new VisualizationItem("UIView", "GetZoomCorners()", new CurvesDrawingVisual([l1, l2, l3 , l4], CrossColor));

        }
    }
}
