using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_Id : MemberAccessorByType<Element>, ICanCreateMemberAccessor
    {
        protected override IEnumerable<LambdaExpression> HandledMembers 
        { 
            get
            { 
                yield return (Element x) => x.Id;
            }
        }

        IMemberAccessor ICanCreateMemberAccessor.Create() => new Element_Id();
        

        protected override bool CanBeSnoooped(Document document, Element value) => false;
        protected override string GetLabel(Document document, Element value) => value.Id.ToString();
    }
}