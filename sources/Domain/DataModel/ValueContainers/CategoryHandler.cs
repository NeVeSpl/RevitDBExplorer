using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class CategoryHandler : TypeHandler<Category>
    {       
        protected override bool CanBeSnoooped(SnoopableContext context, Category category) => category is not null;
        protected override string ToLabel(SnoopableContext context, Category category)
        {
            return $"{category.Name}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Category category)
        {
            yield return new SnoopableObject(document, category);
        }
    }
}