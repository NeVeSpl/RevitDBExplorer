using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class ViewSchedule_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [            
            MemberTemplate<ViewSchedule>.Create((document, target) => TableView.GetAvailableParameters(document, target.Definition.CategoryId), kind: MemberKind.StaticMethod),           
        ];
    }
}