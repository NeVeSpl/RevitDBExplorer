using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class Family_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static Family_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
               SnoopableMemberTemplate<Family>.Create((doc, target) => doc.EditFamily(target), kind: SnoopableMember.Kind.AsArgument),
               SnoopableMemberTemplate<Family>.Create((doc, target) => FamilySizeTableManager.GetFamilySizeTableManager(doc, target.Id), kind: SnoopableMember.Kind.StaticMethod),
               
            }; 
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
