// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System.Reflection
{
    internal static class PropertyInfoExtensions
    {
        /// <summary>
        /// In case that the type has overridden property with only setter, we need to find a getter
        /// </summary>
        public static MethodInfo GetGetGetMethod(this PropertyInfo property)
        {
            // todo : add cache
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