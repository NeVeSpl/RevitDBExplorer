using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Structure.StructuralSections;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Accessors;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class Element_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();

        static Element_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
                SnoopableMemberTemplate<Element>.Create((doc, target) => doc.ActiveView.GetElementOverrides(target.Id), kind: MemberKind.AsArgument),
                SnoopableMemberTemplate<Element>.Create((doc, target) => doc.GetWorksetId(target.Id), kind: MemberKind.AsArgument),
                SnoopableMemberTemplate<FamilyInstance>.Create((doc, target) => StructuralSectionUtils.GetStructuralSection(doc, target.Id), kind: MemberKind.StaticMethod),

                SnoopableMemberTemplate<Reference>.Create((doc, target) => doc.GetElement(target.ElementId).GetGeometryObjectFromReference(target), canBeUsed: x => x.ElementId != null, kind: MemberKind.AsArgument),

#if R2023_MIN
                SnoopableMemberTemplate<Element>.Create((doc, target) => AnalyticalNodeData.GetAnalyticalNodeData(target), kind: MemberKind.StaticMethod, canBeUsed: x => x is ReferencePoint),
                SnoopableMemberTemplate<Element>.Create(typeof(AnalyticalToPhysicalAssociationManager), nameof(AnalyticalToPhysicalAssociationManager.HasAssociation), new AnalyticalToPhysicalAssociationManager_HasAssociation(), kind: MemberKind.AsArgument),
                SnoopableMemberTemplate<Element>.Create(typeof(AnalyticalToPhysicalAssociationManager), nameof(AnalyticalToPhysicalAssociationManager.GetAssociatedElementId), new AnalyticalToPhysicalAssociationManager_GetAssociatedElementId(), kind: MemberKind.AsArgument),
#endif
#if R2024_MIN
                SnoopableMemberTemplate<Element>.Create((doc, target) => AnalyticalToPhysicalAssociationManager.IsAnalyticalElement(doc, target.Id), kind: MemberKind.StaticMethod),
                SnoopableMemberTemplate<Element>.Create((doc, target) => AnalyticalToPhysicalAssociationManager.IsPhysicalElement(doc, target.Id), kind: MemberKind.StaticMethod),
                SnoopableMemberTemplate<Element>.Create(typeof(AnalyticalToPhysicalAssociationManager), nameof(AnalyticalToPhysicalAssociationManager.GetAssociatedElementIds), new AnalyticalToPhysicalAssociationManager_GetAssociatedElementIds(), kind: MemberKind.AsArgument),
#endif

            };
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
