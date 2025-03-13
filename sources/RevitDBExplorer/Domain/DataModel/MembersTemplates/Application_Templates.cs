using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class Application_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [
            MemberTemplate<Autodesk.Revit.ApplicationServices.Application>.Create((doc, category) => FormulaManager.GetFunctions()),
            MemberTemplate<Autodesk.Revit.ApplicationServices.Application>.Create((doc, category) => FormulaManager.GetOperators()),


            MemberTemplate<Autodesk.Revit.ApplicationServices.Application>.Create((doc, category) => OptionalFunctionalityUtils.IsGraphicsAvailable()),
            MemberTemplate<Autodesk.Revit.ApplicationServices.Application>.Create((doc, category) => OptionalFunctionalityUtils.IsIFCAvailable()),
#if R2026_MIN
            MemberTemplate<Autodesk.Revit.ApplicationServices.Application>.Create((doc, category) => OptionalFunctionalityUtils.IsMaterialLibraryAvailable()),
#endif
        ];
    }
}