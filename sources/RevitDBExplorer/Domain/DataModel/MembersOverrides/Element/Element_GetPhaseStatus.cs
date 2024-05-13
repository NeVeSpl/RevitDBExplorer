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
    internal class Element_GetPhaseStatus : MemberAccessorByType<Element>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (Element x, ElementId i) => x.GetPhaseStatus(i) ];


        protected override ReadResult Read(SnoopableContext context, Element element) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(ElementOnPhaseStatus), context.Document.Phases.Size),
            CanBeSnooped = !context.Document.Phases.IsEmpty
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Element element)
        {
            var elementOnPhaseStatuses = context.Document.Phases.OfType<Phase>().Select(x => SnoopableObject.CreateKeyValuePair(context.Document, x, element.GetPhaseStatus(x.Id), "phase:", "status:"));
            return elementOnPhaseStatuses;
        }
    }
}