using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class JoinGeometryUtils_IsCuttingElementInJoin : MemberAccessorTyped<Element>
    {
        public override ReadResult Read(SnoopableContext context, Element element)
        {            
            var elementIds = JoinGeometryUtils.GetJoinedElements(context.Document, element);
            return new ReadResult()
            {
                CanBeSnooped = elementIds.Count > 0,
                Label = $"Elements : {elementIds.Count}",
                ValueTypeName = nameof(JoinGeometryUtils_IsCuttingElementInJoin)
            };

        }

        public override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Element element)
        {
            var elementIds = JoinGeometryUtils.GetJoinedElements(context.Document, element);
            if (elementIds.Any())
            {
                var joinedElements = new FilteredElementCollector(context.Document).WherePasses(new ElementIdSetFilter(elementIds));
                return joinedElements.Select(x => SnoopableObject.CreateInOutPair(context.Document, x, JoinGeometryUtils.IsCuttingElementInJoin(context.Document, element, x)));
            }
            return Enumerable.Empty<SnoopableObject>();
        }
    }
}