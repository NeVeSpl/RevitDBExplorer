using System.Collections.Generic;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal class CategoryNameMapHandler : TypeHandler<CategoryNameMap>
    {    
        protected override bool CanBeSnoooped(SnoopableContext context, CategoryNameMap categoryNameMap) => categoryNameMap?.IsEmpty == false;
        protected override string ToLabel(SnoopableContext context, CategoryNameMap categoryNameMap)
        {
            return Labeler.GetLabelForCollection("Category", categoryNameMap.Size);
        }

        [CodeToString]
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, CategoryNameMap categoryNameMap)
        {
            foreach (Category cat in categoryNameMap)
            {
                yield return new SnoopableObject(context.Document, cat);
            }
        }
    }
}