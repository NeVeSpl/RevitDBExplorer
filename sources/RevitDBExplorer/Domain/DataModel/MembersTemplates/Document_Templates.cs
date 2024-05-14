using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Document_Templates : IHaveMemberTemplates
    {        
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Document>.Create((doc, target) => Document.GetDocumentVersion(target), kind: MemberKind.StaticMethod),
#if R2023_MIN
            MemberTemplate<Document>.Create((doc, target) => target.GetChangedElements(Guid.Empty), kind: MemberKind.Method),
#endif

            MemberTemplate<Document>.Create((doc, target) => BasicFileInfo.Extract(target.PathName), kind: MemberKind.StaticMethod),

            MemberTemplate<Document>.Create((doc, target) => BasePoint.GetSurveyPoint(doc), kind: MemberKind.StaticMethod),
            MemberTemplate<Document>.Create((doc, target) => BasePoint.GetProjectBasePoint(doc), kind: MemberKind.StaticMethod),
            MemberTemplate<Document>.Create((doc, target) => InternalOrigin.Get(doc), kind: MemberKind.StaticMethod),

#if R2024_MIN
            MemberTemplate<Document>.Create((doc, target) => doc.GetUnusedElements(new HashSet<ElementId>()), kind: MemberKind.Method),
            MemberTemplate<Document>.Create((doc, target) => doc.GetAllUnusedElements(new HashSet<ElementId>()), kind: MemberKind.Method),
#endif
        ];        
    }
}