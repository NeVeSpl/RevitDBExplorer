using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersTemplates.Accessors
{
    internal class BoundingBox_BoundingBoxIntersectsFilter : MemberAccessorTypedWithDefaultPresenter<BoundingBoxXYZ>
    {
        protected override ReadResult Read(SnoopableContext context, BoundingBoxXYZ boundingBox)
        {
            var outline = new Outline(boundingBox.Min, boundingBox.Max);
            int count = new FilteredElementCollector(context.Document).WherePasses(new BoundingBoxIntersectsFilter(outline)).GetElementCount();
            return new ReadResult()
            {
                CanBeSnooped = count > 0,
                Label = Labeler.GetLabelForCollection("Element", count),
                AccessorName = nameof(BoundingBox_BoundingBoxIntersectsFilter)
            };
        }

        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, BoundingBoxXYZ boundingBox, IValueContainer state)
        {
            var outline = new Outline(boundingBox.Min, boundingBox.Max);
            var elements = new FilteredElementCollector(context.Document).WherePasses(new BoundingBoxIntersectsFilter(outline)).ToElements();
            return elements.Select(x => new SnoopableObject(context.Document, x));
        }
    }
}
