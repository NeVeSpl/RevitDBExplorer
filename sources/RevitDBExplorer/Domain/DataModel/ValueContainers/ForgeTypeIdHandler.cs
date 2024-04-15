using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ForgeTypeIdHandler : TypeHandler<ForgeTypeId>
    {
        private static readonly Dictionary<string, string> typeIdToPropName = new Dictionary<string, string>();


        static ForgeTypeIdHandler()
        {
            CacheForgeTypeIds(typeof(UnitTypeId));
            CacheForgeTypeIds(typeof(SpecTypeId));
#if R2022_MIN
            CacheForgeTypeIds(typeof(SpecTypeId.Boolean));
            CacheForgeTypeIds(typeof(SpecTypeId.Int));
            CacheForgeTypeIds(typeof(SpecTypeId.Reference));
            CacheForgeTypeIds(typeof(SpecTypeId.String));
            CacheForgeTypeIds(typeof(GroupTypeId));
            CacheForgeTypeIds(typeof(ParameterTypeId));
#endif
        }
        private static void CacheForgeTypeIds(Type  containerType)
        {           
            var props = containerType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            var containerTypeName = containerType.Name;
            if (containerType.IsNested)
            {
                containerTypeName = containerType.DeclaringType.Name + "." + containerType.Name;
            }

            foreach (var prop in props)
            {
                var forgeTypeId = prop.GetValue(null, null) as ForgeTypeId;
                if (forgeTypeId != null)
                {
                    typeIdToPropName[forgeTypeId.TypeId] = $"{containerTypeName}.{prop.Name}";
                }
            }
        }


        protected override bool CanBeSnoooped(SnoopableContext context, ForgeTypeId id) => id is not null;
        protected override string ToLabel(SnoopableContext context, ForgeTypeId id)
        { 
            if (typeIdToPropName.TryGetValue(id.TypeId, out var propName))
            {
                return propName;
            }

            return $"{id.TypeId}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(SnoopableContext context, ForgeTypeId id)
        {
            yield return new SnoopableObject(context.Document, id);
        }
    }
}