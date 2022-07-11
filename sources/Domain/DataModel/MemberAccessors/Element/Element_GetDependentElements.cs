using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_GetDependentElements : MemberAccessorByType<Element>, IHaveFactoryMethod
    {
        public override string MemberName => nameof(Element.GetDependentElements);
        IMemberAccessor IHaveFactoryMethod.Create() => new Element_GetDependentElements();


        protected override bool CanBeSnoooped(Document document, Element element) => true;
        protected override string GetLabel(Document document, Element element) => "[Element]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Element element)
        {
            var ids = element.GetDependentElements(null);
            if (ids.Any())
            {
                var elements = new FilteredElementCollector(document, ids).WhereElementIsNotElementType().ToElements();
                return elements.Select(x => new SnoopableObject(x, document));
            }

            return Enumerable.Empty<SnoopableObject>();
        }
    }
}