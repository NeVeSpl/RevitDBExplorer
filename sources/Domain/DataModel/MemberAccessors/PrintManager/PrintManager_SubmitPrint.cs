using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class PrintManager_SubmitPrint : MemberAccessorByType<PrintManager>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (PrintManager x) => x.SubmitPrint(); yield return (PrintManager x, View v) => x.SubmitPrint(v); } 
      

        protected override bool CanBeSnoooped(Document document, PrintManager value) => false;
        protected override string GetLabel(Document document, PrintManager value) => QuoteGenerator.Deny();
    }
}