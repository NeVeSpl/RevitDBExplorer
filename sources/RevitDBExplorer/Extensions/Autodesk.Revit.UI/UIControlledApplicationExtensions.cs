// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.UI
{
    internal static class UIControlledApplicationExtensions
    {
        public static UIApplication GetUIApplication(this UIControlledApplication uIControlledApplication)
        {
            var type = uIControlledApplication.GetType();
            var field = type.GetField("m_uiapplication", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var value = field.GetValue(uIControlledApplication);

            return value as UIApplication;
        }
    }
}