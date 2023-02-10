using System.Reflection;
using AdW = Autodesk.Windows;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Autodesk.Revit.UI
{
    internal static class RibbonPanelExtensions
    {
        private static readonly FieldInfo RibbonPanelField = typeof(Autodesk.Revit.UI.RibbonPanel).GetField("m_RibbonPanel", BindingFlags.Instance | BindingFlags.NonPublic);
       
        public static AdW.RibbonPanel GetRibbonPanel(this Autodesk.Revit.UI.RibbonPanel panel)
        {
            return RibbonPanelField.GetValue(panel) as AdW.RibbonPanel;
        }
    }
}