// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

using System.Collections.Generic;

namespace System.Reflection
{
    internal static class PropertyInfoExtensions
    {
        private static readonly Dictionary<PropertyInfo, MethodInfo> Cache_GetGetGetMethod = new();

        /// <summary>
        /// In case that the type has overridden property with only setter, we need to find a getter
        /// </summary>
        public static MethodInfo GetGetGetMethod(this PropertyInfo property)
        {
            var getMethod = Cache_GetGetGetMethod.GetOrCreate(property, GetGetGetMethodInternal);
            return getMethod;
        }
        private static MethodInfo GetGetGetMethodInternal(this PropertyInfo property)
        {            
            var declaringType = property.DeclaringType;
            var prop = property;

            while (declaringType != null)
            {
                var result = prop?.GetGetMethod();
                if (result != null)
                {
                    return result;
                }
                declaringType = declaringType.BaseType;
                try
                {
                    prop = declaringType?.GetProperty(property.Name);
                }
                catch
                {

                }
            }

            return null;
        }
    }
}