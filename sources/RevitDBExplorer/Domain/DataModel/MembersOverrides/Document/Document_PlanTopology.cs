using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Document_PlanTopology : MemberAccessorByType<Document>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (Document x, Phase phase) => x.get_PlanTopologies(phase) ];


        protected override ReadResult Read(SnoopableContext context, Document value) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(PlanTopology), null),
            CanBeSnooped = false
        };
       

        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Document value)
        {
            var document = context.Document;
            var transaction = document.IsModifiable == false ? new Transaction(document, GetType().Name) : null;
            transaction?.Start();
            try
            {
                foreach (Phase phase in document.Phases)
                {
                    var topologies = document.get_PlanTopologies(phase).OfType<PlanTopology>().Select(x => new SnoopableObject(document, x));
                    yield return new SnoopableObject(document, phase, topologies);                    
                }
            }
            finally
            {
                transaction?.RollBack();
            }
        }
    }
}