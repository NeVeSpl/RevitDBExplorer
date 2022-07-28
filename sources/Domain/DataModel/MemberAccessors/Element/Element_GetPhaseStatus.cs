using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_GetPhaseStatus : MemberAccessorByType<Element>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Element x, ElementId i) => x.GetPhaseStatus(i); }           


        protected override bool CanBeSnoooped(Document document, Element element) => !document.Phases.IsEmpty;
        protected override string GetLabel(Document document, Element element) => $"[{nameof(ElementOnPhaseStatus)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Element element)
        {
            var elementOnPhaseStatuses = document.Phases.OfType<Phase>().Select(x => SnoopableObject.CreateKeyValuePair(document, x, element.GetPhaseStatus(x.Id), "phase:", "status:"));
            return elementOnPhaseStatuses;
        }
    }
}