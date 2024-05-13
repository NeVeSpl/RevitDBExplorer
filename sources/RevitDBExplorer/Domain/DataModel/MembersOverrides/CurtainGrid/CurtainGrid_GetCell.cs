using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class CurtainGrid_GetCell : MemberAccessorByType<CurtainGrid>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (CurtainGrid x) => x.GetCell(ElementId.InvalidElementId, ElementId.InvalidElementId) ];


        protected override ReadResult Read(SnoopableContext context, CurtainGrid grid) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(CurtainCell), null),
            CanBeSnooped = true
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, CurtainGrid grid)
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

                    yield return new SnoopableObject(context.Document, cell) { Name = $"uGridLineId: {uLineId}, vGridLineId: {vLineId}" };
                }
            }
        }
    }
}
