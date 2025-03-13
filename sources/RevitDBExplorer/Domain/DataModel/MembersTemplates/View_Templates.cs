using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class View_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [       
            MemberTemplate<View>.Create((document, target) => SpatialFieldManager.GetSpatialFieldManager(target), kind: MemberKind.StaticMethod),


#if R2025_MIN
            MemberTemplate<View>.Create((document, target) => RevitLinkGraphicsSettings.IsViewRangeSupported(target)),
#endif
        ];        
    }
}