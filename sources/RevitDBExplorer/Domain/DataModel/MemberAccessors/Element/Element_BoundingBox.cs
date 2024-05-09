﻿using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_BoundingBox : MemberAccessorByFunc<Element, BoundingBoxXYZ>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Element x, View v) => x.get_BoundingBox(v); }


        public Element_BoundingBox() : base((document, element) => element.get_BoundingBox(document.ActiveView), Snoop)
        {

        }


        private static IEnumerable<SnoopableObject> Snoop(Document document, Element element)
        {
            yield return new SnoopableObject(document, null, new[] { new SnoopableObject(document, element.get_BoundingBox(null)) }) { NamePrefix = "view:" };
            if (document.ActiveView != null)
            {
                yield return new SnoopableObject(document, null, new[] { new SnoopableObject(document, element.get_BoundingBox(document.ActiveView)) }) { NamePrefix = "view:", Name = "Active view: " + document.ActiveView.Name };
            }
       
        }
    }
}
