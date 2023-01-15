using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

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
               SnoopableMemberTemplate<Family>.Create((doc, target) => doc.EditFamily(target), kind: MemberKind.AsArgument),
               SnoopableMemberTemplate<Family>.Create((doc, target) => FamilySizeTableManager.GetFamilySizeTableManager(doc, target.Id), kind: MemberKind.StaticMethod),
               
            }; 
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}
