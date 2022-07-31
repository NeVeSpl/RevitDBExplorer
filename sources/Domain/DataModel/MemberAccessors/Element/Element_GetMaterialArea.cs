using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_GetMaterialArea : MemberAccessorByType<Element>, ICanCreateMemberAccessor
    {      
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Element x, ElementId i) => x.GetMaterialArea(i, true); }


        protected override bool CanBeSnoooped(Document document, Element element)
        {
            var paintMaterialIds = element.GetMaterialIds(true);
            var materialIds = element.GetMaterialIds(false);

            return (paintMaterialIds.Count + materialIds.Count) > 0;
        }

        protected override string GetLabel(Document document, Element element)
        {
            return "[double]";
        }

        protected override IEnumerable<SnoopableObject> Snooop(Document document, Element element)
        {
            var paintMaterialIds = element.GetMaterialIds(true);
            var materialIds = element.GetMaterialIds(false);

            foreach (var paintMaterialId in paintMaterialIds)
            {
                yield return SnoopableObject.CreateKeyValuePair(document, paintMaterialId, element.GetMaterialArea(paintMaterialId, true), "paint material:", "area:");
            }
            foreach (var materialId in materialIds)
            {
                yield return SnoopableObject.CreateKeyValuePair(document, materialId, element.GetMaterialArea(materialId, false), "material:", "area:");
            }
        }       
    }
}
