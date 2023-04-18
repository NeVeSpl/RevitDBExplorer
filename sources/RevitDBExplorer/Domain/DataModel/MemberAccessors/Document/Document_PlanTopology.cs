using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Document_PlanTopology : MemberAccessorByType<Document>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Document x, Phase phase) => x.get_PlanTopologies(phase); }       


        protected override bool CanBeSnoooped(Document document, Document value) => false;// document.Phases.Size > 0;
        protected override string GetLabel(Document document, Document value) => $"[{nameof(PlanTopology)}]";

        protected override IEnumerable<SnoopableObject> Snooop(Document document, Document value)
        {
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