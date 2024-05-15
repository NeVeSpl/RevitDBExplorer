using System.Collections.Generic;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;
using RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Schema_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Schema>.WithCustomAC(typeof(Schema), "Get all elements that have entity of this schema", new Schema_GetAllElements(), kind: MemberKind.Extra),
            MemberTemplate<Schema>.WithCustomAC(typeof(Schema), "Erase schema and all entities from the document", new Schema_EraseSchemaAndAllEntities(), kind: MemberKind.Extra),
        ]; 
    }
}