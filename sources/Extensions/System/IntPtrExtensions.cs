using System.Runtime.InteropServices;

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
