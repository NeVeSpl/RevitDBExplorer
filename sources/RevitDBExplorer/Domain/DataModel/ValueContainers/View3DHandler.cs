using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class View3DHandler : TypeHandler<View3D>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, View3D view) => true;

        protected override string ToLabel(SnoopableContext context, View3D view) => $"{view.Name} : ({view.Id})";


        protected override bool CanBeVisualized(SnoopableContext context, View3D view) => true;


        private readonly static Color CrossColor = new Color(255, 0, 225);

        protected override IEnumerable<VisualizationItem> GetVisualization(SnoopableContext context, View3D view)
        {
            var orientation = view.GetOrientation();

            yield return new VisualizationItem("View", "Origin", new CrossDrawingVisual(view.Origin, VisualizationItem.Accent1Color) { LineLength = 0.1 });
            yield return new VisualizationItem("View3D", "GetOrientation().EyePosition", new CrossDrawingVisual(orientation.EyePosition, VisualizationItem.Accent2Color) { LineLength = 0.1 });
            yield return new VisualizationItem("View3D", "GetOrientation().ForwardDirection", new VectorDrawingVisual(orientation.EyePosition, orientation.EyePosition+ orientation.ForwardDirection*100, VisualizationItem.NormalColor));          

        }
    }
}
