using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class BoundingBoxXYZType : Base.ValueType<BoundingBoxXYZ>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new BoundingBoxXYZType();
        }


        protected override bool CanBeSnoooped(BoundingBoxXYZ box) => box is not null;
        protected override string ToLabel(BoundingBoxXYZ box)
        {
            return $"Min({box.Min.X:0.##}, {box.Min.Y:0.##}, {box.Min.Z:0.##}), Max({box.Max.X:0.##}, {box.Max.Y:0.##}, {box.Max.Z:0.##})";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, BoundingBoxXYZ box)
        {
            yield return new SnoopableObject(box, document);
        }
    }
}