using System.Runtime.InteropServices;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace System
{
    internal static class IntPtrExtensions
    {
        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);


        public static void BringWindowToFront(this IntPtr handle)
        {
            SetForegroundWindow(handle);
        }



        public const int WM_KEYDOWN = 0x0100;

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);


        public static int PostKeyMessage(this IntPtr handle, int key)
        {
            var result = PostMessage(handle, WM_KEYDOWN, key, 0);

            return result;
        }
    }
}