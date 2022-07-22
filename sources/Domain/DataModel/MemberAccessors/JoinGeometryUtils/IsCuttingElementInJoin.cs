using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class JoinGeometryUtils_IsCuttingElementInJoin : MemberAccessorTyped<Element>
    {
        public override ReadResult Read(Document document, Element element)
        {            
            var elementIds = JoinGeometryUtils.GetJoinedElements(document, element);
            return new ReadResult()
            {
                CanBeSnooped = elementIds.Count > 0,
                Value = $"Elements : {elementIds.Count}",
                ValueTypeName = nameof(JoinGeometryUtils_IsCuttingElementInJoin)
            };

        }

        public override IEnumerable<SnoopableObject> Snoop(Document document, Element element)
        {
            var elementIds = JoinGeometryUtils.GetJoinedElements(document, element);
            if (elementIds.Any())
            {
                var joinedElements = new FilteredElementCollector(document).WherePasses(new ElementIdSetFilter(elementIds));
                return joinedElements.Select(x => SnoopableObject.CreateInOutPair(document, x, JoinGeometryUtils.IsCuttingElementInJoin(document, element, x)));
            }
            return Enumerable.Empty<SnoopableObject>();
        }
    }
}
