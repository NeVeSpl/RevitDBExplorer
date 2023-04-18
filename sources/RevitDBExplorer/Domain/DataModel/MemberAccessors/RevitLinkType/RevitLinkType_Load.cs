using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class RevitLinkType_Load : MemberAccessorByType<RevitLinkType>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() 
        {
            yield return (RevitLinkType x) => x.Load(); 
            yield return (RevitLinkType x) => x.Reload(); 
        }


        protected override bool CanBeSnoooped(Document document, RevitLinkType value) => false;
        protected override string GetLabel(Document document, RevitLinkType value) => QuoteGenerator.Deny();
    }
}