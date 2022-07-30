using System.Reflection;

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
                var propertyValue = property?.GetGetGetMethod()?.Invoke(target, default)?.ToString();
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    return propertyValue;
                }
            }
            return null;
        }
    }
}