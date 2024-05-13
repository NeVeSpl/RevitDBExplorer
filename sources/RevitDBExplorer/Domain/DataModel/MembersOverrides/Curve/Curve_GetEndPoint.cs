using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Curve_GetEndPoint : MemberAccessorByType<Curve>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (Curve x) => x.GetEndPoint(0) ];


        protected override ReadResult Read(SnoopableContext context, Curve curve) => new()
        {
            Label = GetLabel(curve),
            CanBeSnooped = curve.IsBound
        };        
        private static string GetLabel(Curve curve)
        {
            var p0 = curve.GetEndPoint(0);
            var p1 = curve.GetEndPoint(1);

            string value = $"({p0.X:0.##}, {p0.Y:0.##}, {p0.Z:0.##}) - ({p1.X:0.##}, {p1.Y:0.##}, {p1.Z:0.##})";
            return value;
        }


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Curve curve)
        {
            var p0 = curve.GetEndPoint(0);
            var p1 = curve.GetEndPoint(1);

            yield return new SnoopableObject(context.Document, p0) { Index = 0, Name = "Start"};
            yield return new SnoopableObject(context.Document, p1) { Index = 1, Name = "End" };
        }
    }
}