// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.DB
{
    internal static class ElementIdExtensions
    {
        public static long Value(this ElementId id)
        {
#if R2023e
            return id.IntegerValue;
#endif
#if R2024b
            return id.Value;
#endif
        }
    }
    internal static class ElementIdFactory
    { 
        public static ElementId Create(long id)
        {
#if R2023e
            return new ElementId((int)id);
#endif
#if R2024b
            return new ElementId(id);
#endif
        }

    }
}