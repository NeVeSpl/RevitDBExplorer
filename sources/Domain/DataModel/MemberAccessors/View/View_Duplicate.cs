using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class View_Duplicate : MemberAccessorByType<View>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (View x) => x.Duplicate(ViewDuplicateOption.Duplicate); }         

      
        protected override bool CanBeSnoooped(Document document, View value) => false;
        protected override string GetLabel(Document document, View value) => QuoteGenerator.Deny();       
    }
}