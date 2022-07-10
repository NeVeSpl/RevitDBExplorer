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
    }
}