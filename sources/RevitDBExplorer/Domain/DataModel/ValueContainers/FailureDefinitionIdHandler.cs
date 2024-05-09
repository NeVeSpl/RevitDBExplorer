using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class FailureDefinitionIdHandler : TypeHandler<FailureDefinitionId>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, FailureDefinitionId value)
        {
            return true;
        }
        protected override string ToLabel(SnoopableContext context, FailureDefinitionId value)
        {
            return $"FailureDefinitionId ({value.Guid})";
        }

        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, FailureDefinitionId value)
        {
            var failure = ControlledApplication.GetFailureDefinitionRegistry().FindFailureDefinition(value);
            yield return new SnoopableObject(context.Document, failure);
        }
    }
}
