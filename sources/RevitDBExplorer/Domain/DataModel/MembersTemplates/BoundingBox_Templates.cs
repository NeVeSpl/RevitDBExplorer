using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;
using RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class BoundingBox_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<BoundingBoxXYZ>.WithCustomAC(typeof(BoundingBoxXYZ), "BoundingBoxIntersectsFilter", new BoundingBox_BoundingBoxIntersectsFilter(), kind: MemberKind.Extra, documentationFactoryMethod: () => new DocXml() { Summary ="TEST" }),
        ]; 
    }
}
