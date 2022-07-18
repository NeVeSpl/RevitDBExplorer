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
    }
}