using System.Collections.Generic;
using Autodesk.Revit.DB;
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
        ]; 
    }
}