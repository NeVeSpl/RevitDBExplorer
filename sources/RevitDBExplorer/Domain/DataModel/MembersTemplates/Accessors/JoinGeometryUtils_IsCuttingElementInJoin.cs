using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors
{
    internal class JoinGeometryUtils_IsCuttingElementInJoin : MemberAccessorTypedWithDefaultPresenter<Element>
    {
        protected override ReadResult Read(SnoopableContext context, Element element)
        {            
            var elementIds = JoinGeometryUtils.GetJoinedElements(context.Document, element);
            return new ReadResult()
            {
                CanBeSnooped = elementIds.Count > 0,
                Label = $"Elements : {elementIds.Count}",
                AccessorName = nameof(JoinGeometryUtils_IsCuttingElementInJoin)
            };

        }

        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Element element, IValueContainer state)
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