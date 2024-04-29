using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class Document_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();

        static Document_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
                SnoopableMemberTemplate<Document>.Create((doc, target) => Document.GetDocumentVersion(target), kind: MemberKind.StaticMethod),
#if R2023_MIN
                SnoopableMemberTemplate<Document>.Create((doc, target) => target.GetChangedElements(Guid.Empty), kind: MemberKind.Method),
#endif

                SnoopableMemberTemplate<Document>.Create((doc, target) => BasicFileInfo.Extract(target.PathName), kind: MemberKind.StaticMethod),

                SnoopableMemberTemplate<Document>.Create((doc, target) => BasePoint.GetSurveyPoint(doc), kind: MemberKind.StaticMethod),
                SnoopableMemberTemplate<Document>.Create((doc, target) => BasePoint.GetProjectBasePoint(doc), kind: MemberKind.StaticMethod),

            };
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
