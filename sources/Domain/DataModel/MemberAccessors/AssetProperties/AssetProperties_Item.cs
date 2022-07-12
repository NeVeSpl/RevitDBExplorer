using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md
namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class AssetProperties_Item : MemberAccessorByType<AssetProperties>, IHaveFactoryMethod
    {
        public override string MemberName => "Item";
        public override string MemberParams => typeof(int).Name;

        IMemberAccessor IHaveFactoryMethod.Create() => new AssetProperties_Item();


        protected override bool CanBeSnoooped(Document document, AssetProperties assetProperties) => assetProperties.Size > 0;
        protected override string GetLabel(Document document, AssetProperties assetProperties) => "[AssetProperty]";
        protected override IEnumerable<SnoopableObject> Snooop(Document document, AssetProperties assetProperties)
        {            
            for (int i = 0; i < assetProperties.Size; i++)
            {
                yield return new SnoopableObject(assetProperties[i], document);
            }
        }
    }
}