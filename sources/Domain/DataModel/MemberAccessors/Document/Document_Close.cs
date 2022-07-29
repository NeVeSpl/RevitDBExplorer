using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Document_Close : MemberAccessorByType<Document>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Document x) => x.Close(); yield return (Document x) => x.Close(true); }         

      
        protected override bool CanBeSnoooped(Document document, Document value) => false;
        protected override string GetLabel(Document document, Document value) => "'cannot be done' - Colin";       
    }
}