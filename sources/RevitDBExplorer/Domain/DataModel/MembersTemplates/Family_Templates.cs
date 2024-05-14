using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Family_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static Family_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
               MemberTemplate<Family>.Create((doc, target) => doc.EditFamily(target), kind: MemberKind.AsArgument),
               MemberTemplate<Family>.Create((doc, target) => FamilySizeTableManager.GetFamilySizeTableManager(doc, target.Id), kind: MemberKind.StaticMethod),
               
            }; 
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
