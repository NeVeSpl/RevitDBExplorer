using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_GetPhaseStatus : IMemberAccessor, IHaveFactoryMethod
    {
        string IHaveFactoryMethod.TypeAndMemberName => "Element.GetPhaseStatus";
        IMemberAccessor IHaveFactoryMethod.Create() => new Element_GetPhaseStatus();

        public ReadResult Read(Document document, object @object)
        {
            bool canBeSnooped = !document.Phases.IsEmpty;
            return new ReadResult("[ElementOnPhaseStatus]", "ElementOnPhaseStatus", canBeSnooped);
        }

        public IEnumerable<SnoopableObject> Snooop(Document document, object @object)
        {
            var element = @object as Element;

            var elementOnPhaseStatuses = document.Phases.OfType<Phase>().Select(x => new SnoopableObject(element.GetPhaseStatus(x.Id), document, x.Name));
            return elementOnPhaseStatuses;
        }
    }
}
