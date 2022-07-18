using System.Collections.Generic;
using System.Linq;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System
{
    internal static class TypeExtensions
    {
        public static int NumberOfBaseTypes(this Type type)
        {
            int i = 0;
            while (type != null)
            {
                type = type.BaseType;
                i++;
            }
            return i;
        }

        public static object GetDefaultValue(this Type type)
        {
            if ((type.IsValueType) && (type != typeof(void)))
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }


        static readonly Dictionary<Type, string> primitiveTypes = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" },
            { typeof(void), "void" },
        };


        /// <summary>
        /// source: https://stackoverflow.com/questions/2579734/get-the-type-name
        /// </summary>
        public static string GetCSharpName(this Type type)
        {
            string result;
            if (primitiveTypes.TryGetValue(type, out result))
            {
                return result;
            }
            else
            {
                result = type.Name.Replace('+', '.');
            }

            if (!type.IsGenericType)
            {
                return result;
            }
            else if (type.IsNested && type.DeclaringType.IsGenericType)
            {
                return String.Empty;
#if DEBUG
                throw new NotImplementedException();
#endif
            }

            result = result.Substring(0, result.IndexOf("`"));
            return result + "<" + string.Join(", ", type.GetGenericArguments().Select(GetCSharpName)) + ">";
        }
    }
}