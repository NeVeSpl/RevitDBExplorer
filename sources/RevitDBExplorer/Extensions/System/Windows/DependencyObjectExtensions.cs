using System.Windows.Input;
using System.Windows.Media;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

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


        public static T GetParentOfType<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            var pointer = dependencyObject;
            var type = typeof(T);
            while (pointer != null)
            {
                if (pointer is T)
                {
                    return pointer as T;
                }
                pointer = VisualTreeHelper.GetParent(pointer);
            }
            return null;
        }
   
        public static FrameworkElement GetParent(this DependencyObject dependencyObject, Func<FrameworkElement, bool> predicate)
        {
            var pointer = dependencyObject;          
            while (pointer != null)
            {
                if ((pointer is FrameworkElement frameworkElement) && (predicate(frameworkElement)))
                {
                    return frameworkElement;
                }
                pointer = VisualTreeHelper.GetParent(pointer);
            }
            return null;
        }

        public static object GetDataContext(this DependencyObject dependencyObject)
        {
            if (dependencyObject is FrameworkContentElement frameworkContentElement)
            {
                return frameworkContentElement.DataContext;
            }
            if (dependencyObject is FrameworkElement frameworkElement)
            {
                return frameworkElement.DataContext;
            }
            return null;
        }
     }
}