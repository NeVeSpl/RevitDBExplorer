using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class FailureHandlingOptions_ : MemberAccessorByType<FailureHandlingOptions>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() 
        { 
            yield return (FailureHandlingOptions x) => x.SetClearAfterRollback(false);
            yield return (FailureHandlingOptions x) => x.SetDelayedMiniWarnings(false);
            yield return (FailureHandlingOptions x) => x.SetFailuresPreprocessor(null);
            yield return (FailureHandlingOptions x) => x.SetForcedModalHandling(false);
            yield return (FailureHandlingOptions x) => x.SetTransactionFinalizer(null);
        }         

      
        protected override bool CanBeSnoooped(Document document, FailureHandlingOptions value) => false;
        protected override string GetLabel(Document document, FailureHandlingOptions value) => QuoteGenerator.Deny();       
    }
}