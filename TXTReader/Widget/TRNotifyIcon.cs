using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TXTReader.Utility;

namespace TXTReader.Widget {
    class TRNotifyIcon {
        private NotifyIcon ni;
        public TRNotifyIcon() {
            ni = new NotifyIcon();
            ni.Icon = ni.Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            ni.Visible = true;
            MenuItem toggle = new MenuItem("显示/隐藏界面");
            ni.DoubleClick += ni_DoubleClick;
            toggle.Click += toggle_Click;
            MenuItem exit = new MenuItem("退出");
            exit.Click += exit_Click;
            MenuItem[] childen = new MenuItem[] { toggle, exit };
            ni.ContextMenu = new ContextMenu(childen);
            G.KeyHook.Hook_Start();
            G.KeyHook.KeyDownHook += KeyHook_KeyHook;
        }

        void KeyHook_KeyHook(int keyCode) {
            if (keyCode == (int)Keys.T && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Control) {
                G.MainWindow.Toggle();
            }
        }

        void ni_DoubleClick(object sender, EventArgs e) {
            G.MainWindow.Toggle();
        }

        void toggle_Click(object sender, EventArgs e) {            
            G.MainWindow.Toggle();
        }

        void exit_Click(object sender, EventArgs e) {
            G.MainWindow.Close();
        }

        public void Close() {
            G.KeyHook.Hook_Clear();
            ni.Dispose();            
        }
    }
}
