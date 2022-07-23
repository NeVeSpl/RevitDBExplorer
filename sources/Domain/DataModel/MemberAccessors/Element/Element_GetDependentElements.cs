using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_GetDependentElements : MemberAccessorByType<Element>, ICanCreateMemberAccessor
    {       
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (Element x, ElementFilter ef) => x.GetDependentElements(ef); } }
        IMemberAccessor ICanCreateMemberAccessor.Create() => new Element_GetDependentElements();


        protected override bool CanBeSnoooped(Document document, Element element) => true;
        protected override string GetLabel(Document document, Element element) => $"[{nameof(Element)}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Element element)
        {
            var ids = element.GetDependentElements(null);
            if (ids.Any())
            {
                var elements = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(ids)).ToElements();
                return elements.Select(x => new SnoopableObject(document, x));
            }

            return Enumerable.Empty<SnoopableObject>();
        }
    }
}