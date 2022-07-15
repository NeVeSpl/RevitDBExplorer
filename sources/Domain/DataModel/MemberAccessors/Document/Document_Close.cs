using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Document_Close : MemberAccessorByType<Document>, IHaveFactoryMethod
    {        
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (Document x) => x.Close(); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new Document_Close();

      
        protected override bool CanBeSnoooped(Document document, Document value) => false;
        protected override string GetLabel(Document document, Document value) => "'cannot be done' - Colin";       
    }
}