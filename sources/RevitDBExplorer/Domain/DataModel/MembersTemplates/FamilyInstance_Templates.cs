using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class FamilyInstance_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<FamilyInstance>.Create((doc, target) => AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(target), canBeUsed: (x)=> AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(x) , kind: MemberKind.StaticMethod),
        ]; 
    }
}