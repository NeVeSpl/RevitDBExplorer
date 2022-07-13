using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Curve_GetEndPoint : MemberAccessorByType<Curve>, IHaveFactoryMethod
    {
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (Curve x) => x.GetEndPoint(0); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new Curve_GetEndPoint();


        protected override bool CanBeSnoooped(Document document, Curve curve) => curve.IsBound;
        protected override string GetLabel(Document document, Curve curve)
        {
            var p0 = curve.GetEndPoint(0);
            var p1 = curve.GetEndPoint(1);

            string value = $"({p0.X:0.##}, {p0.Y:0.##}, {p0.Z:0.##}) - ({p1.X:0.##}, {p1.Y:0.##}, {p1.Z:0.##})";
            return value;
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Curve curve)
        {
            var p0 = curve.GetEndPoint(0);
            var p1 = curve.GetEndPoint(1);

            yield return new SnoopableObject(p0, document, "[0] Start");
            yield return new SnoopableObject(p1, document, "[1] End");
        }
    }
}