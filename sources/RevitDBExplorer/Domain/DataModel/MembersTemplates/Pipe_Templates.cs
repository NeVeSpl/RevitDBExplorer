using System.Collections.Generic;
using Autodesk.Revit.DB.Plumbing;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Pipe_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Pipe>.Create((doc, target) => PlumbingUtils.HasOpenConnector(doc, target.Id), kind: MemberKind.StaticMethod),
           
        ];
    }
}
