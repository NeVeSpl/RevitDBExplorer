using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure.StructuralSections;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class StructuralSectionContainer : Base.ValueContainer<StructuralSection>
    {
        protected override bool CanBeSnoooped(StructuralSection value)
        {
            return true;
        }

        protected override string ToLabel(StructuralSection value)
        {
            return value.ToString();
        }

        protected override IEnumerable<SnoopableObject> Snooop(Document document, StructuralSection value)
        {
            yield return new SnoopableObject(document, value);
        }
    }
}
