using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Accessors;
using RevitDBExplorer.Domain.DataModel.MemberTemplates.Base;
using RevitDBExplorer.Domain.DataModel.Streams;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberTemplates
{
    internal class Schema_Templates : IHaveMemberTemplates
    {
        private static readonly IEnumerable<ISnoopableMemberTemplate> templates = Enumerable.Empty<ISnoopableMemberTemplate>();


        static Schema_Templates()
        {
            templates = new ISnoopableMemberTemplate[]
            {
                SnoopableMemberTemplate<Schema>.Create(typeof(Schema), "Get all elements that have entity of this schema", new Schema_GetAllElements(), kind: MemberKind.Extra),
                SnoopableMemberTemplate<Schema>.Create(typeof(Schema), "Erase schema and all entities from the document", new Schema_EraseSchemaAndAllEntities(), kind: MemberKind.Extra),
            };            
        }


        public IEnumerable<ISnoopableMemberTemplate> GetTemplates()
        {
            return templates;
        }
    }
}