using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class DialogBoxShowingEventArgs_OverrideResult : MemberAccessorByType<DialogBoxShowingEventArgs>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (DialogBoxShowingEventArgs x) => x.OverrideResult(0); }         

      
        protected override bool CanBeSnoooped(Document document, DialogBoxShowingEventArgs value) => false;
        protected override string GetLabel(Document document, DialogBoxShowingEventArgs value) => QuoteGenerator.Deny();       
    }
}