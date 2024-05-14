using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Structure.StructuralSections;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;
using RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Element_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [   
            MemberTemplate<Element>.Create((doc, target) => doc.ActiveView.GetElementOverrides(target.Id), kind: MemberKind.AsArgument),
            MemberTemplate<Element>.Create((doc, target) => doc.GetWorksetId(target.Id), kind: MemberKind.AsArgument),
            MemberTemplate<FamilyInstance>.Create((doc, target) => StructuralSectionUtils.GetStructuralSection(doc, target.Id), kind: MemberKind.StaticMethod),

            MemberTemplate<Reference>.Create((doc, target) => doc.GetElement(target.ElementId).GetGeometryObjectFromReference(target), canBeUsed: x => x.ElementId != null, kind: MemberKind.AsArgument),

#if R2023_MIN
            MemberTemplate<Element>.Create((doc, target) => AnalyticalNodeData.GetAnalyticalNodeData(target), kind: MemberKind.StaticMethod, canBeUsed: x => x is ReferencePoint),
            MemberTemplate<Element>.Create(typeof(AnalyticalToPhysicalAssociationManager), nameof(AnalyticalToPhysicalAssociationManager.HasAssociation), new AnalyticalToPhysicalAssociationManager_HasAssociation(), kind: MemberKind.AsArgument),
            MemberTemplate<Element>.Create(typeof(AnalyticalToPhysicalAssociationManager), nameof(AnalyticalToPhysicalAssociationManager.GetAssociatedElementId), new AnalyticalToPhysicalAssociationManager_GetAssociatedElementId(), kind: MemberKind.AsArgument),
#endif
#if R2024_MIN
            MemberTemplate<Element>.Create((doc, target) => AnalyticalToPhysicalAssociationManager.IsAnalyticalElement(doc, target.Id), kind: MemberKind.StaticMethod),
            MemberTemplate<Element>.Create((doc, target) => AnalyticalToPhysicalAssociationManager.IsPhysicalElement(doc, target.Id), kind: MemberKind.StaticMethod),
            MemberTemplate<Element>.Create(typeof(AnalyticalToPhysicalAssociationManager), nameof(AnalyticalToPhysicalAssociationManager.GetAssociatedElementIds), new AnalyticalToPhysicalAssociationManager_GetAssociatedElementIds(), kind: MemberKind.AsArgument),
#endif
        ];
    }
}
