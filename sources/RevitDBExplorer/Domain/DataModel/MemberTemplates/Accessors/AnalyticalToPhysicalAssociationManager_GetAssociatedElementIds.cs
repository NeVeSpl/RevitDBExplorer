using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates.Accessors
{
#if R2024_MIN
    internal class AnalyticalToPhysicalAssociationManager_GetAssociatedElementIds : MemberAccessorByFunc<Element, ISet<ElementId>>
    {
        public AnalyticalToPhysicalAssociationManager_GetAssociatedElementIds() : base(GetAssociatedElementIds)
        {

        }


        private static ISet<ElementId> GetAssociatedElementIds(Document document, Element element)
        {
            var analyticalToPhysicalmanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(document);
            var result = analyticalToPhysicalmanager.GetAssociatedElementIds(element.Id);
            return result;
        }
    }
#endif
}
