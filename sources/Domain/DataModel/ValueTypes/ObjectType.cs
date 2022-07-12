using System;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueTypes
{
    internal sealed class ObjectType : Base.ValueType<object>
    {
        private readonly Type type;

        public ObjectType(Type type)
        {
            this.type = type;
        }


        public override string TypeName => base.TypeName != "Object" ? base.TypeName : $"Object({type.Name})";
        protected override bool CanBeSnoooped(object @object) => @object is not null;
        protected override string ToLabel(object @object)
        {
            string name = GetNameForObjectFromProperty(@object);
            string typeName = @object.GetType()?.Name;     
            return $"{typeName}: {name}";
        }
        protected override IEnumerable<SnoopableObject> Snooop(Document document, object @object)
        {
            yield return new SnoopableObject(@object, document);
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