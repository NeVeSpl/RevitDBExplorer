using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class RevitLinkType_Load : MemberAccessorByType<RevitLinkType>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() =>
        [
             (RevitLinkType x) => x.Load(),
             (RevitLinkType x) => x.Reload(),
        ];


        protected override ReadResult Read(SnoopableContext context, RevitLinkType revitLinkType) => ReadResult.Forbidden;       
    }
}