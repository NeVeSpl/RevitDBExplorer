using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueTypes.Base;

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class ForgeTypeIdType : Base.ValueType<ForgeTypeId>, IHaveFactoryMethod
    {
        IValueType IHaveFactoryMethod.Create()
        {
            return new ForgeTypeIdType();
        }


        protected override bool CanBeSnoooped(ForgeTypeId id) => true;
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
