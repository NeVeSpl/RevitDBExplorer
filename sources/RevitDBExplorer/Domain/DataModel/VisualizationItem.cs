using Autodesk.Revit.DB;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class VisualizationItem
    {
        public readonly static Color FaceColor = new Color(80, 175, 228);
        public readonly static Color NormalColor = new Color(106, 13, 173);
        public readonly static Color XAxisColor = new Color(255, 0, 0);
        public readonly static Color YAxisColor = new Color(0, 255, 0);
        public readonly static Color ZAxisColor = new Color(0, 0, 255);
        public readonly static Color SolidColor = new Color(80, 175, 228);
        public readonly static Color StartColor = new Color(0, 255, 0);
        public readonly static Color EndColor = new Color(255, 0, 0);
        public readonly static Color CurveColor = new Color(80, 175, 228);
        public readonly static Color PointColor = new Color(0, 0, 255);
        public readonly static Color Accent1Color = new Color(0, 255, 255);      
        public readonly static Color Accent2Color = new Color(255, 0, 255);
        public readonly static Color Accent3Color = new Color(255, 255, 0);


        public string Group { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DrawingVisual DrawingVisual { get; set; }


        public VisualizationItem(DrawingVisual drawingVisual)
        {
            DrawingVisual = drawingVisual;
        }

        public VisualizationItem(string group, string name, DrawingVisual drawingVisual)
        {
            Group = group;
            Name = name;
            DrawingVisual = drawingVisual;
            Description = drawingVisual.ToString();
        }
    }
}