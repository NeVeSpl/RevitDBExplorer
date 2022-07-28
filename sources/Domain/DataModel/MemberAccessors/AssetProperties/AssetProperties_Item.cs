using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class AssetProperties_Item : MemberAccessorByType<AssetProperties>, ICanCreateMemberAccessor
    { 
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (AssetProperties x) => x[0]; }     


        protected override bool CanBeSnoooped(Document document, AssetProperties assetProperties) => assetProperties.Size > 0;
        protected override string GetLabel(Document document, AssetProperties assetProperties) => $"[{nameof(AssetProperty)} : {assetProperties.Size}]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, AssetProperties assetProperties)
        {            
            for (int i = 0; i < assetProperties.Size; i++)
            {
                yield return new SnoopableObject(document, assetProperties[i]);
            }
        }
    }
}