// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.DB
{
    internal  static class GeometryObjectExtensions
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
    }
}