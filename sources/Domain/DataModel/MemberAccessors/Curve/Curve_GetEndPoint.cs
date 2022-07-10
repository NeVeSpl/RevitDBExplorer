using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Curve_GetEndPoint : IMemberAccessor, IHaveFactoryMethod
    {
        string IHaveFactoryMethod.TypeAndMemberName => "Curve.GetEndPoint";
        IMemberAccessor IHaveFactoryMethod.Create() => new Curve_GetEndPoint();


        public ReadResult Read(Document document, object @object)
        {
            var curve = (Curve) @object;
            bool canBeSnooped = curve is not null && curve.IsBound;

            var p0 = curve.GetEndPoint(0);
            var p1 = curve.GetEndPoint(1);

            string value = $"({p0.X:0.##}, {p0.Y:0.##}, {p0.Z:0.##}) - ({p1.X:0.##}, {p1.Y:0.##}, {p1.Z:0.##})";

            return new ReadResult(value, "XYZ", canBeSnooped);
        }

        public IEnumerable<SnoopableObject> Snooop(Document document, object @object)
        {
            var curve = (Curve)@object;

            var p0 = curve.GetEndPoint(0);
            var p1 = curve.GetEndPoint(1);

            yield return new SnoopableObject(p0, document, "[0] Start");
            yield return new SnoopableObject(p1, document, "[1] End");
        }
    }
}
