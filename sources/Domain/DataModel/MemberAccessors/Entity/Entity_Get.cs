using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Entity_Get : MemberAccessorByType<Entity>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() { yield return (Entity x, Field f) => x.Get<object>(f); }         


        protected override bool CanBeSnoooped(Document document, Entity entity)
        {
            return entity.IsValid() && entity.Schema.ReadAccessGranted() && entity.Schema.ListFields().Count > 0;
        }
        protected override string GetLabel(Document document, Entity entity)
        {
            return "[Extensible storage field values]";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, Entity entity)
        {
            var fields = entity.Schema.ListFields();
            var getWithField = ((Func<Field, object>)entity.Get<object>).Method.GetGenericMethodDefinition();
            var getWithFielAndUnit = ((Func<Field, ForgeTypeId, object>)entity.Get<object>).Method.GetGenericMethodDefinition();

            List<SnoopableObject> snoopableFields = new List<SnoopableObject>(fields.Count);

            foreach (var field in fields)
            {
                var fieldValueType = GetFieldValueType(field);
                var fieldSpecType = field.GetSpecTypeId();
                MethodInfo constructedGenericGet = null;
                object[] parameters = null;              
                bool isMeasurableSpec = IsMeasurableSpec(fieldSpecType);

                if (isMeasurableSpec || fieldSpecType == SpecTypeId.Custom)
                {
                    var unit = isMeasurableSpec ? UnitUtils.GetValidUnits(fieldSpecType).FirstOrDefault() : UnitTypeId.Custom;
                    constructedGenericGet = getWithFielAndUnit.MakeGenericMethod(fieldValueType);
                    parameters = new object[] { field, unit };
                }
                else
                {
                    constructedGenericGet = getWithField.MakeGenericMethod(fieldValueType);
                    parameters = new object[] { field };
                }
                var value = constructedGenericGet.Invoke(entity, parameters);
                yield return SnoopableObject.CreateKeyValuePair(document, field, value, keyPrefix :"");
            }
        }

        private bool IsMeasurableSpec(ForgeTypeId id)
        {
#if R2022b
            return UnitUtils.IsMeasurableSpec(id);
#endif
#if R2021e
            try
            {
                if (UnitUtils.IsSpec(id))
                {
                    UnitUtils.GetValidUnits(id);
                    return true;
                }
            }
            catch
            {

            }
            return false;
#endif
        }

        private Type GetFieldValueType(Field field)
        {
            return field.ContainerType switch
            {
                ContainerType.Simple => field.ValueType,
                ContainerType.Array => typeof(IList<>).MakeGenericType(field.ValueType),
                ContainerType.Map => typeof(IDictionary<,>).MakeGenericType(field.KeyType, field.ValueType),
                _ => throw new NotImplementedException(),
            };
        }
    }
}