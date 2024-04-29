using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class CurtainGrid_GetCell : MemberAccessorByType<CurtainGrid>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (CurtainGrid x) => x.GetCell(ElementId.InvalidElementId, ElementId.InvalidElementId); }


        protected override bool CanBeSnoooped(Document document, CurtainGrid grid) => true;
        protected override string GetLabel(Document document, CurtainGrid grid)
        {      
            string value = $"[CurtainCell]";
            return value;
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, CurtainGrid grid)
        {
            var uLineIds = grid.GetUGridLineIds();
            var vLineIds = grid.GetVGridLineIds();
            uLineIds.Add(ElementId.InvalidElementId);
            vLineIds.Add(ElementId.InvalidElementId);

            foreach (var uLineId in uLineIds)
            {
                foreach (var vLineId in vLineIds)
                {
                    var cell = grid.GetCell(uLineId, vLineId);

                    yield return new SnoopableObject(document, cell) { Name = $"uGridLineId: {uLineId}, vGridLineId: {vLineId}" };
                }
            }
        }
    }
}
