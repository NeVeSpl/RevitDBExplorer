using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates.Accessors
{
#if R2023b
    internal class AnalyticalToPhysicalAssociationManager_HasAssociation : MemberAccessorByFunc<Element, bool>
    {
        public AnalyticalToPhysicalAssociationManager_HasAssociation() : base(HasAssociation)
        {

        }


        private static bool HasAssociation(Document document, Element element)
        {
            var analyticalToPhysicalmanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            var result = analyticalToPhysicalmanager.HasAssociation(element.Id);
            return result;

        }
    }
#endif
}
