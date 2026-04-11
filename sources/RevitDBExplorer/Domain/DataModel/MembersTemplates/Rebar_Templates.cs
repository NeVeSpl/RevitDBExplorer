using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Rebar_Templates : IHaveMemberTemplates
    {

        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            
#if R2025_MIN
            MemberTemplate<Rebar>.Create((doc, target) => RebarSpliceUtils.GetSpliceChain(target)),
#endif                
        ];
    }
}