using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class WorksharingUtils_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static WorksharingUtils_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
               SnoopableMemberTemplate<Element>.Create((doc, target) => WorksharingUtils.GetCheckoutStatus(doc, target.Id), kind: SnoopableMember.Kind.StaticMethod),
               SnoopableMemberTemplate<Element>.Create((doc, target) => WorksharingUtils.GetModelUpdatesStatus(doc, target.Id), kind: SnoopableMember.Kind.StaticMethod),
               SnoopableMemberTemplate<Element>.Create((doc, target) => WorksharingUtils.GetWorksharingTooltipInfo(doc, target.Id), kind: SnoopableMember.Kind.StaticMethod),
            };
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
