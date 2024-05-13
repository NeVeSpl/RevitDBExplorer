using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class IndependentTag_TagText : MemberAccessorByType<IndependentTag>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (IndependentTag x) => x.TagText ];


        protected override ReadResult Read(SnoopableContext context, IndependentTag independentTag) => new()
        {
            Label = GetLabel(independentTag),
            CanBeSnooped = false
        };
        private static string GetLabel(IndependentTag independentTag)
        {
#if R2025_MIN
            if (RebarBendingDetail.IsBendingDetail(independentTag))
            {
                return string.Empty;
            }
#endif

            return independentTag.TagText;
        }
    }
}