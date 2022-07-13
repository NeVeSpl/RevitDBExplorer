using System.Linq;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System.Reflection
{
    internal static class MethodInfoExtensions
    {
        
        public static string GetUniqueId(this MethodInfo methodInfo)
        {
            var signature = String.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.Name).ToArray());
            var id = $"{methodInfo.DeclaringType.Name}.{methodInfo.Name}({signature})";
            return id;
        }
    }
}