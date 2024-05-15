using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;
using RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class HostObject_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [   
               MemberTemplate<HostObject>.Create((doc, target) => HostObjectUtils.GetTopFaces(target), kind: MemberKind.StaticMethod),
               MemberTemplate<HostObject>.Create((doc, target) => HostObjectUtils.GetBottomFaces(target), kind: MemberKind.StaticMethod),
               MemberTemplate<HostObject>.WithCustomAC(typeof(HostObjectUtils), "GetSideFaces", new HostObjectUtils_GetSideFaces(), kind: MemberKind.StaticMethod ),           
        ]; 
    }
}