using System.Reflection;
using RevitDBExplorer.Domain.DataModel.Members.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System
{
    internal static class ObjectExtensions
    {
        public static string TryGetPropertyValue(this object target, params string[] propertyNames)
        {            
            foreach (var propName in propertyNames)
            {
                var property = target.GetType()?.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var getMethod = property?.GetGetGetMethod();

                if (getMethod != null)
                {
                    var @params = getMethod.GetParameters();
                    if (@params.Length == 0)
                    {
                        var factory = GenericFactory.GetInstance(getMethod.DeclaringType, getMethod.ReturnType);
                        var func = factory.CreateCompiledLambda(getMethod);
                        var propertyValue = func(target)?.ToString();

                        if (!string.IsNullOrEmpty(propertyValue))
                        {
                            return propertyValue;
                        }
                    }
                }
               
            }
            return null;
        }

        public static T CastValue<T>(this object value, Type type)
        {
            if (value == null)
            {
                return (T)type.GetDefaultValue();
            }
            else
            {
                return (T)value;
            }
        }
    }
}