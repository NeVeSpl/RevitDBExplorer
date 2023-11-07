using System.Collections.Generic;
using System.Linq;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System.Reflection
{
    internal static class MethodInfoExtensions
    {
        private static readonly Dictionary<MethodInfo, string> Cache_UniqueId = new();

        public static string GetUniqueId(this MethodInfo methodInfo)
        {
            var uniqueId = Cache_UniqueId.GetOrCreate(methodInfo, CreateUniqueId);          
            return uniqueId;
        }
        private static string CreateUniqueId(this MethodInfo methodInfo)
        {            
            var signature = String.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.Name).ToArray());
            var id = $"{methodInfo.DeclaringType.Name}.{methodInfo.Name}({signature})";
            return id;
        }


        public static string GenerateInvocation(this MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            bool isProp = parameters.Length == 0 && methodInfo.IsSpecialName;
            if (!isProp)
            {
                var signature = String.Join(", ", methodInfo.GetParameters().Select(p => $"{p.Name}: {GetDefaultValue(p)}").ToArray());
                return $"{methodInfo.Name}({signature})";
            }
            
            return $"{methodInfo.Name.Substring(4)}";
        }

        private static string GetDefaultValue(ParameterInfo parameterInfo)
        {
            var value = parameterInfo.ParameterType.GetDefaultValue();

            if (value == null)
            {
                return "null";
            }
            if (value is bool)
            {
                return "false";
            }
            if (value is Enum enumValue)
            {
                return $"{value.GetType().Name}.{enumValue}";
            }

            
            return value.ToString();
        }
    }
}