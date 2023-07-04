using Autodesk.Windows;
using RevitDBExplorer.Domain;

namespace RevitDBExplorer
{
    public static class ApplicationModifyTab
    {
        private static Autodesk.Windows.RibbonPanel ribbonPanel;
        public static void Init(Autodesk.Windows.RibbonPanel ribbonPanel, bool visible = false)
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
            if (ApplicationModifyTab.ribbonPanel is null) return;

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