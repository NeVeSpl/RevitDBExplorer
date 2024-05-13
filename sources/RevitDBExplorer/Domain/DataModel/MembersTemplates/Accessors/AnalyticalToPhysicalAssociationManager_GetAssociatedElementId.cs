using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors
{
#if R2023_MIN
    internal class AnalyticalToPhysicalAssociationManager_GetAssociatedElementId : MemberAccessorByFunc<Element, ElementId>
    {
        public AnalyticalToPhysicalAssociationManager_GetAssociatedElementId() : base(GetAssociatedElementId)
        {

        }


        private static ElementId GetAssociatedElementId(Document document, Element element)
        {
            var analyticalToPhysicalmanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            var result = analyticalToPhysicalmanager.GetAssociatedElementId(element.Id);
            return result;

        }
    }
#endif
}
