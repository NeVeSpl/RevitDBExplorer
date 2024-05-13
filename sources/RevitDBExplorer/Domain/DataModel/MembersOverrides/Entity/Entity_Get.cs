using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.Members;
using RevitDBExplorer.Domain.DataModel.Members.Accessors;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.MembersOverrides
{
    internal class Entity_Get : MemberAccessorByType<Entity>, ICanCreateMemberAccessor
    {
        IEnumerable<LambdaExpression> ICanCreateMemberAccessor.GetHandledMembers() => [ (Entity x, Field f) => x.Get<object>(f) ];


        protected override ReadResult Read(SnoopableContext context, Entity entity) => new()
        {
            Label = "[Extensible storage field values]",
            CanBeSnooped = entity.IsValid() && entity.Schema.ReadAccessGranted() && entity.Schema.ListFields().Count > 0
        };


        protected override IEnumerable<SnoopableObject> Snoop(SnoopableContext context, Entity entity)
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
                yield return SnoopableObject.CreateKeyValuePair(context.Document, field, value, keyPrefix :"");
            }
        }

        private static bool IsMeasurableSpec(ForgeTypeId id)
        {
#if R2022_MIN
            return UnitUtils.IsMeasurableSpec(id);
#endif
#if R2021_MAX
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

        private static Type GetFieldValueType(Field field)
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