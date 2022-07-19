using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class ForgeTypeIdType : Base.ValueType<ForgeTypeId>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new ForgeTypeIdType();
        }


        protected override bool CanBeSnoooped(ForgeTypeId id) => id is not null;
        protected override string ToLabel(ForgeTypeId id)
        {
            return $"{id.TypeId}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, ForgeTypeId id)
        {
            yield return new SnoopableObject(id, document);
        }
    }
}