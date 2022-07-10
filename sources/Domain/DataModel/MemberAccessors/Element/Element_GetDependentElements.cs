using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_GetDependentElements : IMemberAccessor, IHaveFactoryMethod
    {
        string IHaveFactoryMethod.TypeAndMemberName => "Element.GetDependentElements";
        IMemberAccessor IHaveFactoryMethod.Create() => new Element_GetDependentElements();

        public ReadResult Read(Document document, object @object)
        {
            return new ReadResult("[Element]", "IList<ElementId>", true);
        }

        public IEnumerable<SnoopableObject> Snooop(Document document, object @object)
        {
            var element = @object as Element;
            var ids = element.GetDependentElements(null);
            if  (ids.Any())
            {
                var elements = new FilteredElementCollector(document, ids).WhereElementIsNotElementType().ToElements();
                return elements.Select(x => new SnoopableObject(x, document));
            }

            return Enumerable.Empty<SnoopableObject>();
        }
    }
}