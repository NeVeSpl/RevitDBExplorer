using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_GetDependentElements : MemberAccessorByFunc<Element, IList<ElementId>>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Element x, ElementFilter ef) => x.GetDependentElements(ef); }


        public Element_GetDependentElements() : base( (document, element) => element.GetDependentElements(null) )
        {

        }
    }
}