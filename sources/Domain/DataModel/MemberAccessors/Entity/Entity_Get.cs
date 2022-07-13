using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md


namespace RevitDBExplorer.Domain.DataModel.MemberAccessors
{
    internal class Entity_Get : MemberAccessorByType<Entity>, IHaveFactoryMethod
    {       
        protected override IEnumerable<LambdaExpression> HandledMembers { get { yield return (Entity x, Field f) => x.Get<object>(f); } }
        IMemberAccessor IHaveFactoryMethod.Create() => new Entity_Get();



        protected override bool CanBeSnoooped(Document document, Entity entity)
        {
            return entity.IsValid() && entity.Schema.ReadAccessGranted() && entity.Schema.ListFields().Count > 0;
        }

        protected override string GetLabel(Document document, Entity entity)
        {
            return "[Extensible storage values]";
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
                MethodInfo closedGenericGet = null;
                object[] parameters = null;
                if (UnitUtils.IsMeasurableSpec(fieldSpecType) || fieldSpecType == SpecTypeId.Custom)
                {
                    var unit = UnitUtils.IsMeasurableSpec(fieldSpecType) ? UnitUtils.GetValidUnits(fieldSpecType)[0] : UnitTypeId.Custom;
                    closedGenericGet = getWithFielAndUnit.MakeGenericMethod(fieldValueType);
                    parameters = new object[] { field, unit };
                }
                else
                {
                    closedGenericGet = getWithField.MakeGenericMethod(fieldValueType);
                    parameters = new object[] { field };
                }
                var value = closedGenericGet.Invoke(entity, parameters);
                yield return new SnoopableObject(field, document, null, new[] { new SnoopableObject(value, document) });
            }
        }

        private Type GetFieldValueType(Field field)
        {
            return field.ContainerType switch
            {
                ContainerType.Simple => field.ValueType,
                ContainerType.Array => typeof(IList<>).MakeGenericType(field.ValueType),
                ContainerType.Map => typeof(IDictionary<,>).MakeGenericType(field.KeyType, field.ValueType),
                _ => throw new NotSupportedException(),
            };
        }
    }
}