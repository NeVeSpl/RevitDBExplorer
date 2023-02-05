// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

using System.Windows.Media;

namespace System.Windows
{
    internal static class DependencyObjectExtensions
    {
        public static Window GetParentWindow(this DependencyObject dependencyObject)
        {
            var pointer = dependencyObject;
            while (pointer != null && !(pointer is Window))
            {
                pointer = LogicalTreeHelper.GetParent(pointer);
            }
            return pointer as Window;
        }



        public static bool IsParentFor(this DependencyObject dependencyObject, DependencyObject child)
        {
            var pointer = child;
            while (pointer != null)
            {
                if (pointer == dependencyObject) return true;
                pointer = VisualTreeHelper.GetParent(pointer);
            }
            pointer = child;
            while (pointer != null)
            {
                if (pointer == dependencyObject) return true;
                pointer = LogicalTreeHelper.GetParent(pointer);
            }
            return false;
        }
    }
}