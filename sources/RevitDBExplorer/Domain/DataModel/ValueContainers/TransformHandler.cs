using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class TransformHandler : TypeHandler<Transform>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, Transform transform) => true;

        protected override string ToLabel(SnoopableContext context, Transform transform)
        {
            string id = "";
            if (transform.IsIdentity)
            {
                id = "Identity";
            }
            if (transform.IsTranslation)
            {
                id = "Translation";
            }

            return $"Transform: {id}";
        }

        protected override IEnumerable<SnoopableObject> Snooop(Document document, Transform transform)
        {
            yield return new SnoopableObject(document, transform);
        }
    }
}