using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Element_GetMaterialVolume : MemberAccessorByType<Element>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (Element x, ElementId i) => x.GetMaterialVolume(i) ];


        protected override ReadResult Read(SnoopableContext context, Element element) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(Double), null),
            CanBeSnooped = CanBeSnoooped(element),
        };
        private static bool CanBeSnoooped(Element element)
        {
            var paintMaterialIds = element.GetMaterialIds(true);
            var materialIds = element.GetMaterialIds(false);

            return (paintMaterialIds.Count + materialIds.Count) > 0;
        }


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Element element)
        {
            var paintMaterialIds = element.GetMaterialIds(true);
            var materialIds = element.GetMaterialIds(false);
            
            foreach (var materialId in paintMaterialIds.Concat(materialIds))
            {
                yield return SnoopableObject.CreateKeyValuePair(context.Document, materialId, element.GetMaterialVolume(materialId), "material:", "volume:");
            }
        }       
    }
}