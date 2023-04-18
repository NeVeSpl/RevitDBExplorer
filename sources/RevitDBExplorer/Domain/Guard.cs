using System;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class Guard
    {
        public static void IsAssignableToType<T>(object value)
        {
            if (value is T)
            {
                return;
            }

            throw new TypeAccessException("It should not be possible to be here, but here we are.");
        }
    }
}