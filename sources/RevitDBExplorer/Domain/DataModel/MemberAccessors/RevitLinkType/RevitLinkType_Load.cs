using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class RevitLinkType_Load : MemberAccessorByType<RevitLinkType>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() =>
        [
             (RevitLinkType x) => x.Load(),
             (RevitLinkType x) => x.Reload(),
        ];


        public override ReadResult Read(SnoopableContext context, RevitLinkType revitLinkType) => ReadResult.Forbidden;       
    }
}