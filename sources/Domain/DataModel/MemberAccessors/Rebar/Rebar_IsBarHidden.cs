using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Rebar_IsBarHidden : MemberAccessorByType<Rebar>, IHaveFactoryMethod
    {
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (Rebar x, View v) => x.IsBarHidden(v, 7); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new Rebar_IsBarHidden();


        protected override bool CanBeSnoooped(Document document, Rebar rebar) => true;
        protected override string GetLabel(Document document, Rebar rebar) => "[bool]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Rebar rebar)
        {
            for (int i = 0; i < rebar.NumberOfBarPositions; ++i)
            {
                var result = rebar.IsBarHidden(document.ActiveView, i);
                yield return new SnoopableObject(null, document, new[] { new SnoopableObject(result, document) }) { Name = $"{i}" };
            }
        }
    }
}
