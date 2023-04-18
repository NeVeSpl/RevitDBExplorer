using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Element_GetMaterialVolume : MemberAccessorByType<Element>, ICanCreateMemberAccessor
    {      
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Element x, ElementId i) => x.GetMaterialVolume(i); }


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
            
            foreach (var materialId in paintMaterialIds.Concat(materialIds))
            {
                yield return SnoopableObject.CreateKeyValuePair(document, materialId, element.GetMaterialVolume(materialId), "material:", "volume:");
            }
        }       
    }
}