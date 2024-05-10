using System.Collections.Generic;
using Autodesk.Revit.DB.Structure.StructuralSections;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class StructuralSectionHandler : TypeHandler<StructuralSection>
    {
        protected override bool CanBeSnoooped(SnoopableContext context, StructuralSection value)
        {
            return true;
        }

        protected override string ToLabel(SnoopableContext context, StructuralSection value)
        {
            return value.ToString();
        }

    }
}
