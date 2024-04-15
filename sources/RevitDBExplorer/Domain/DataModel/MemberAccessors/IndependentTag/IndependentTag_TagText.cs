using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class IndependentTag_TagText : MemberAccessorByType<IndependentTag>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers()
        {
            yield return (IndependentTag x) => x.TagText;            
        }      
        

        protected override bool CanBeSnoooped(Document document, IndependentTag value) => false;
        protected override string GetLabel(Document document, IndependentTag value)
        {
#if R2025_MIN
            if (RebarBendingDetail.IsBendingDetail(value))
            {
                return string.Empty;
            }
#endif

            return value.TagText;
        }
    }
}