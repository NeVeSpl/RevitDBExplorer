using Autodesk.Windows;
using RevitDBExplorer.Domain;

namespace RevitDBExplorer.Utils
{
    internal static class ApplicationModifyTab
    {
        private static RibbonPanel ribbonPanel;


        public static void Init(RibbonPanel ribbonPanel, bool visible = false)
        {
            ApplicationModifyTab.ribbonPanel = ribbonPanel.Clone();
            UpdateInternal(visible);
        }

        public static void Update(bool visible)
        {
            ExternalExecutor.ExecuteInRevitContextAsync(uiapp => { UpdateInternal(visible); });
        }

        private static void UpdateInternal(bool visible)
        {
            if (ribbonPanel is null) return;

            var tab = ComponentManager.Ribbon.FindTab("Modify");
            if (tab is null) return;

            tab.Panels.Remove(ribbonPanel);

            if (visible)
            {
                tab.Panels.Add(ribbonPanel);
            }
        }
    }
}