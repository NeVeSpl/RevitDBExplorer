using System;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueContainers
{
    internal sealed class ObjectContainer : Base.ValueContainer<object>
    {
        private readonly Type type;


        public ObjectContainer(Type type)
        {
            this.type = type;
        }

        // in case if a value is null, we use ObjectType knowledge about the value type
        public override string TypeName => base.TypeName != "Object" ? base.TypeName : $"Object : {type.GetCSharpName()}";
        protected override bool CanBeSnoooped(object @object) => @object is not null;
        protected override string ToLabel(object @object)
        {
            string name = GetNameForObjectFromProperty(@object);
            string typeName = @object.GetType()?.GetCSharpName();     
            return $"{typeName}: {name}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, object @object)
        {
            yield return new SnoopableObject(document, @object);
        }


        private static readonly string[] propertyThatContainsName = new[]  { "Name", "Title", "SchemaName", "FieldName" };
        private static string GetNameForObjectFromProperty(object obj)
        {
            foreach (var propName in propertyThatContainsName)
            {
                var property = obj.GetType()?.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var propertyValue = property?.GetGetGetMethod()?.Invoke(obj, default) as string;
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    return propertyValue;
                }
            }
            return null;
        }
    }
}