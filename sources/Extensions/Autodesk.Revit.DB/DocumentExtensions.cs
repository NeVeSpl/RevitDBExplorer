// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.DB
{
    internal static class DocumentExtensions
    {
        public static object GetElementOrCategory(this Document document, ElementId id)
        {
            var element = document.GetElement(id);
            if (element != null)
            {
                return element;
            }
           
            var category = Category.GetCategory(document, id);
            if (category != null)
            {
                return category;
            }
            
            return null;
        }
    }
}