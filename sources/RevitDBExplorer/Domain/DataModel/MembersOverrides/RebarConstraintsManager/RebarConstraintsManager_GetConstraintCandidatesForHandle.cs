using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using Autodesk.Revit.DB.Structure;
using System.Linq.Expressions;
using RevitDBExplorer.Domain.DataModel.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class RebarConstraintsManager_GetConstraintCandidatesForHandle : MemberAccessorByType<RebarConstraintsManager>, ICanCreateMemberAccessor
    {
        public IEnumerable<LambdaExpression> GetHandledMembers() => [ (RebarConstraintsManager x) => x.GetConstraintCandidatesForHandle(null, ElementId.InvalidElementId)];


        protected override ReadResult Read(SnoopableContext context, RebarConstraintsManager manager) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(RebarConstraint), null),
            CanBeSnooped = true
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, RebarConstraintsManager manager)
        {
            var handles = manager.GetAllHandles();

            foreach (var handle in handles)
            {
                x.GetConstraintCandidatesForHandle

                    yield return new SnoopableObject(context.Document, cell) { Name = $"uGridLineId: {uLineId}, vGridLineId: {vLineId}" };
                
            }
        }

    }
}
