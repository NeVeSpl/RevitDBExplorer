// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System.Collections.Generic
{
    internal static class IDictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> create)   
        {
            if (!dict.TryGetValue(key, out TValue val))
            {
                val = create(key);
                dict.Add(key, val);
            }

            return val;
        }
    }
}