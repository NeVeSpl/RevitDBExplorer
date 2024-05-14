using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Schema_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static Schema_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
                MemberTemplate<Schema>.Create(typeof(Schema), "Get all elements that have entity of this schema", new Schema_GetAllElements(), kind: MemberKind.Extra),
                MemberTemplate<Schema>.Create(typeof(Schema), "Erase schema and all entities from the document", new Schema_EraseSchemaAndAllEntities(), kind: MemberKind.Extra),
            };            
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}