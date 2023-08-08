using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class EnumHandler<TEnumType> : TypeHandler<TEnumType> where TEnumType : System.Enum
    {
        protected override bool CanBeSnoooped(SnoopableContext context, TEnumType enumValue) => true;
        protected override string ToLabel(SnoopableContext context, TEnumType enumValue)
        {
            return $"{enumValue?.GetType()?.Name}.{enumValue}";
        }

        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, TEnumType value)
        {
            yield return new SnoopableObject(context.Document, value?.GetType() ?? typeof(TEnumType));
        }
    }
}