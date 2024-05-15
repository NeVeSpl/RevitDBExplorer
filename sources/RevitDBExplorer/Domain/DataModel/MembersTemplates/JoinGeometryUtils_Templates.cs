using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;
using RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class JoinGeometryUtils_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Element>.Create((doc, target) => JoinGeometryUtils.GetJoinedElements(doc, target)),
            MemberTemplate<Element>.WithCustomAC(typeof(JoinGeometryUtils), nameof(JoinGeometryUtils.IsCuttingElementInJoin), new JoinGeometryUtils_IsCuttingElementInJoin())
        ];       
    }
}