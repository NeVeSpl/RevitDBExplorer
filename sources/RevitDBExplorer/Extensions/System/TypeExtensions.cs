using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
                if (type == typeof(int))
                {
                    return default(int);
                }
                if (type == typeof(long))
                {
                    return default(long);
                }
                if (type == typeof(float))
                {
                    return default(float);
                }
                if (type == typeof(double))
                {
                    return default(double);
                }                
                if (type == typeof(bool))
                {
                    return default(bool);
                }
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

        private static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>()
        {
          { nameof(Byte), "byte" }, 
          { nameof(Int32), "int" },
          { nameof(Int64), "long" },
          { nameof(Single), "float" },
          { nameof(Double), "double" },
          { nameof(Decimal), "decimal" },
          { nameof(Object), "object" },
          { nameof(Boolean), "bool" },
          { nameof(Char), "char" },
          { nameof(String), "string" } 
        };

        public static string ReduceTypeName(this string typeName)
        {
            if (Aliases.TryGetValue(typeName, out var result)) 
            {
                return result;
            }
            return typeName;
        }

        public static bool IsPrimitiveTypeName(this string typeName)
        {
            return primitiveTypes.Values.Contains(typeName, StringComparer.OrdinalIgnoreCase);
        }


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


        public static Func<T> CompileFactoryMethod<T>(this Type type)
        {
            var newExpression = Expression.New(type);
            var factoryLambda = Expression.Lambda<Func<T>>(newExpression);
            var factoryMethod = factoryLambda.Compile();
            return factoryMethod;
        }
    }
}