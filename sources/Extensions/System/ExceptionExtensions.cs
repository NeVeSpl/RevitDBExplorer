using Autodesk.Revit.UI;
using RevitDBExplorer.Domain;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System
{
    internal static class ExceptionExtensions
    {
        public static void ShowErrorMsg(this Exception ex, string title)
        {
            var exType = ex.GetType();
            var typeName =  $"{exType.Namespace}.{ex.GetType().GetCSharpName()}";
            var dialog = new TaskDialog(title)
            {
                MainInstruction = $"{typeName}:\r\n{Labeler.GetLabelForException(ex)}",
                MainContent = ex.StackTrace,
                CommonButtons = TaskDialogCommonButtons.Ok,
                DefaultButton = TaskDialogResult.Ok,
                MainIcon = TaskDialogIcon.TaskDialogIconError
            };
            dialog.Show();
        }
    }
}