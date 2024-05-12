using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB.Visual;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class AssetProperties_Item : MemberAccessorTypedWithDefaultPresenter<AssetProperties>, ICanCreateMemberAccessor
    { 
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (AssetProperties x) => x[0]; }


        public override ReadResult Read(SnoopableContext context, AssetProperties assetProperties) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(AssetProperty), assetProperties.Size),
            CanBeSnooped = assetProperties.Size > 0
        };

        public override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, AssetProperties assetProperties, IValueContainer state)
        {            
            for (int i = 0; i < assetProperties.Size; i++)
            {
                yield return new SnoopableObject(context.Document, assetProperties[i]);
            }
        }
    }
}