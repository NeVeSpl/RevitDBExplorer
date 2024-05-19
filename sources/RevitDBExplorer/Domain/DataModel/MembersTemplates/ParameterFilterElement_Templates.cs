using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates
{
    internal class ParameterFilterElement_Templates : IHaveMemberTemplates
    {
        public IEnumerable<ISnoopableMemberTemplate> GetTemplates() =>
        [

            MemberTemplate<ParameterFilterElement>.Create((doc, target) => doc.ActiveView.GetFilterOverrides(target.Id), kind: MemberKind.AsArgument),
            MemberTemplate<SelectionFilterElement>.Create((doc, target) => doc.ActiveView.GetFilterOverrides(target.Id), kind: MemberKind.AsArgument),
            MemberTemplate<ParameterFilterElement>.Create((doc, target) => doc.ActiveView.GetFilterVisibility(target.Id), kind: MemberKind.AsArgument),
            MemberTemplate<SelectionFilterElement>.Create((doc, target) => doc.ActiveView.GetFilterVisibility(target.Id), kind: MemberKind.AsArgument),
            MemberTemplate<ParameterFilterElement>.Create((doc, target) => doc.ActiveView.GetIsFilterEnabled(target.Id), kind: MemberKind.AsArgument),
            MemberTemplate<SelectionFilterElement>.Create((doc, target) => doc.ActiveView.GetIsFilterEnabled(target.Id), kind: MemberKind.AsArgument),
            MemberTemplate<ParameterFilterElement>.Create((doc, target) => doc.ActiveView.IsFilterApplied(target.Id), kind: MemberKind.AsArgument),
            MemberTemplate<SelectionFilterElement>.Create((doc, target) => doc.ActiveView.IsFilterApplied(target.Id), kind: MemberKind.AsArgument),
        ];
    }
}