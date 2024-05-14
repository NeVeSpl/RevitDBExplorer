using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class WorksharingUtils_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Element>.Create((doc, target) => WorksharingUtils.GetCheckoutStatus(doc, target.Id), kind: MemberKind.StaticMethod),
            MemberTemplate<Element>.Create((doc, target) => WorksharingUtils.GetModelUpdatesStatus(doc, target.Id), kind: MemberKind.StaticMethod),
            MemberTemplate<Element>.Create((doc, target) => WorksharingUtils.GetWorksharingTooltipInfo(doc, target.Id), kind: MemberKind.StaticMethod),
        ];
    }
}