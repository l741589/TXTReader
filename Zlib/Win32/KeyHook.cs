using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zlib.Win32 {

    public delegate void KeyHookEventHandler(int keyCode);

    public class KeyHook {
        //委托 
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        static int hHook = 0;
        public const int WH_KEYBOARD_LL = 13;
        //LowLevel键盘截获，如果是WH_KEYBOARD＝2，并不能对系统键盘截取，Acrobat Reader会在你截取之前获得键盘。 
        HookProc KeyBoardHookProcedure;
        //键盘Hook结构函数 
        [StructLayout(LayoutKind.Sequential)]
        public class KeyBoardHookStruct {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        //设置钩子 
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //抽掉钩子 
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll")]
        //调用下一个钩子 
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        public event KeyHookEventHandler KeyDownHook;
        public event KeyHookEventHandler KeyUpHook;

        // 安装键盘钩子 
        public void Hook_Start() {
            if (hHook == 0) {
                hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyBoardHookProcedure = KeyBoardHookProc,
                        GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                if (hHook == 0) {
                    Hook_Clear();
                }
            }
        }

        //取消钩子事件 
        public void Hook_Clear() {
            bool retKeyboard = true;
            if (hHook != 0) {
                retKeyboard = UnhookWindowsHookEx(hHook);
                hHook = 0;
            }
            if (!retKeyboard) throw new Exception("UnhookWindowsHookEx failed.");
        }

        public int KeyBoardHookProc(int nCode, int wParam, IntPtr lParam) {
            if (nCode >= 0) {
                KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
                if (KeyDownHook!=null&&(wParam == MessageConst.WM_KEYDOWN || wParam == MessageConst.WM_SYSKEYDOWN)) KeyDownHook(kbh.vkCode);
                if (KeyUpHook!=null&&(wParam == MessageConst.WM_KEYUP || wParam == MessageConst.WM_SYSKEYUP)) KeyUpHook(kbh.vkCode);
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }
    }
}
