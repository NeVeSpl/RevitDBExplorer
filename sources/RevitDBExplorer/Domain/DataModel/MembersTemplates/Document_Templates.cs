using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Document_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Document>.Create((doc, target) => BasicFileInfo.Extract(target.PathName), kind: MemberKind.StaticMethod),
            MemberTemplate<Document>.Create((doc, target) => BasePoint.GetSurveyPoint(doc), kind: MemberKind.StaticMethod),
            MemberTemplate<Document>.Create((doc, target) => BasePoint.GetProjectBasePoint(doc), kind: MemberKind.StaticMethod),
            MemberTemplate<Document>.Create((doc, target) => InternalOrigin.Get(doc), kind: MemberKind.StaticMethod),

            MemberTemplate<Document>.Create((doc, target) => GlobalParametersManager.AreGlobalParametersAllowed(doc)),
            MemberTemplate<Document>.Create((doc, target) => GlobalParametersManager.GetAllGlobalParameters(doc)),
            MemberTemplate<Document>.Create((doc, target) => GlobalParametersManager.GetGlobalParametersOrdered(doc)),

            MemberTemplate<Document>.Create((doc, target) => LightGroupManager.GetLightGroupManager(doc)),
#if R2022_MIN
            MemberTemplate<Document>.Create((doc, target) => TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc)),
#endif
#if R2023_MIN
            MemberTemplate<Document>.Create((doc, target) => AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(doc)),
#endif
        ];
    }
}