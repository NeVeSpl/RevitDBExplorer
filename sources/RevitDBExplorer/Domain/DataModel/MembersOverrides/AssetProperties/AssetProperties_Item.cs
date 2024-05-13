using System.Collections.Generic;
using System.Linq.Expressions;
using Autodesk.Revit.DB.Visual;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class AssetProperties_Item : MemberAccessorByType<AssetProperties>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (AssetProperties x) => x[0] ];


        protected override ReadResult Read(SnoopableContext context, AssetProperties assetProperties) => new()
        {
            Label = Labeler.GetLabelForCollection(nameof(AssetProperty), assetProperties.Size),
            CanBeSnooped = assetProperties.Size > 0
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, AssetProperties assetProperties)
        {            
            for (int i = 0; i < assetProperties.Size; i++)
            {
                yield return new SnoopableObject(context.Document, assetProperties[i]);
            }
        }
    }
}