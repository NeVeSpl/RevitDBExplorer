using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Family_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Family>.Create((doc, target) => doc.EditFamily(target), kind: MemberKind.AsArgument),
            MemberTemplate<Family>.Create((doc, target) => FamilySizeTableManager.GetFamilySizeTableManager(doc, target.Id), kind: MemberKind.StaticMethod),               

            MemberTemplate<Family>.Create((doc, target) => FamilyUtils.FamilyCanConvertToFaceHostBased(doc, target.Id), kind: MemberKind.StaticMethod),
            
        ]; 
    }
}