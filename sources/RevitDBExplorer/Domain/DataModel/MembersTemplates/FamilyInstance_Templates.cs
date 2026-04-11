using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Structure.StructuralSections;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;
using RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class FamilyInstance_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [

            MemberTemplate<FamilyInstance>.Create((doc, target) => StructuralSectionUtils.GetStructuralSection(doc, target.Id), kind: MemberKind.StaticMethod),
            MemberTemplate<FamilyInstance>.WithCustomAC(typeof(StructuralSectionUtils), nameof(StructuralSectionUtils.GetStructuralElementDefinitionData), new StructuralSectionUtils_GetStructuralElementDefinitionData()),

            MemberTemplate<FamilyInstance>.Create((doc, target) => AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(target), kind: MemberKind.StaticMethod),
            MemberTemplate<FamilyInstance>.Create((doc, target) => AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(target), canBeUsed: (x)=> AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(x) , kind: MemberKind.StaticMethod),


            MemberTemplate<FamilyInstance>.Create((doc, target) => MassLevelData.IsMassFamilyInstance(doc, target.Id)),

            MemberTemplate<FamilyInstance>.Create((doc, target) => MassInstanceUtils.GetMassLevelDataIds(doc, target.Id), canBeUsed: (target) => MassLevelData.IsMassFamilyInstance(target.Document, target.Id)),
            MemberTemplate<FamilyInstance>.Create((doc, target) => MassInstanceUtils.GetMassLevelIds(doc, target.Id), canBeUsed: (target) => MassLevelData.IsMassFamilyInstance(target.Document, target.Id)),
            MemberTemplate<FamilyInstance>.Create((doc, target) => MassInstanceUtils.GetGrossFloorArea(doc, target.Id), canBeUsed: (target) => MassLevelData.IsMassFamilyInstance(target.Document, target.Id)),
            MemberTemplate<FamilyInstance>.Create((doc, target) => MassInstanceUtils.GetGrossSurfaceArea(doc, target.Id), canBeUsed: (target) => MassLevelData.IsMassFamilyInstance(target.Document, target.Id)),
            MemberTemplate<FamilyInstance>.Create((doc, target) => MassInstanceUtils.GetGrossVolume(doc, target.Id), canBeUsed: (target) => MassLevelData.IsMassFamilyInstance(target.Document, target.Id)),
            MemberTemplate<FamilyInstance>.Create((doc, target) => MassInstanceUtils.GetJoinedElementIds(doc, target.Id), canBeUsed: (target) => MassLevelData.IsMassFamilyInstance(target.Document, target.Id)),
            
        ]; 
    }
}