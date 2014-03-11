using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Zlib.Win32 {
    public static class Win32Util {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        public static bool SetForegroundWindow(this Window w) {
            HwndSource source = (HwndSource)PresentationSource.FromVisual(w);
            IntPtr handle = source.Handle;
            return SetForegroundWindow(handle);
        }

        public static IntPtr SetFocus(this Window w) {
            HwndSource source = (HwndSource)PresentationSource.FromVisual(w);
            IntPtr handle = source.Handle;
            return SetFocus(handle);
        }
    }
}
