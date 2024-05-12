using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitDBExplorer.Domain.DataModel.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class IndependentTag_TagText : MemberAccessorByType<IndependentTag>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (IndependentTag x) => x.TagText ];


        public override ReadResult Read(SnoopableContext context, IndependentTag independentTag) => new()
        {
            Label = GetLabel(independentTag),
            CanBeSnooped = false
        };
        private string GetLabel(IndependentTag independentTag)
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