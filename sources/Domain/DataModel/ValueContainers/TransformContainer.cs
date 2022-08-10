using System.Collections.Generic;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class TransformContainer : Base.ValueContainer<Transform>
    {
        protected override bool CanBeSnoooped(Transform transform) => true;

        protected override string ToLabel(Transform transform)
        {
            string id = transform.IsIdentity ? "Identity" : "";
            return $"Transform: {id}";
        }

        protected override IEnumerable<SnoopableObject> Snooop(Document document, Transform transform)
        {
            yield return new SnoopableObject(document, transform);
        }
    }
}