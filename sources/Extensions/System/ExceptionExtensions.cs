using Autodesk.Revit.UI;
using RevitDBExplorer.Domain;

namespace System
{
    internal static class ExceptionExtensions
    {
        public static void ShowErrorMsg(this Exception ex, string title)
        {
            var dialog = new TaskDialog(title)
            {
                MainInstruction = Labeler.GetLabelForException(ex),
                MainContent = ex.StackTrace,
                CommonButtons = TaskDialogCommonButtons.Ok,
                DefaultButton = TaskDialogResult.Ok,
                MainIcon = TaskDialogIcon.TaskDialogIconError
            };
            dialog.Show();
        }
    }
}