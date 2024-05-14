using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class FamilyInstance_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();

        static FamilyInstance_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
               MemberTemplate<FamilyInstance>.Create((doc, target) => AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(target), canBeUsed: (x)=> AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(x) , kind: MemberKind.StaticMethod),
            }; 
        }

        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
