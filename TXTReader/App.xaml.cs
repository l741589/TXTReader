﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TXTReader.Utility;
using TXTReader.Data;
using Microsoft.Win32;
using System.Security.Permissions;

namespace TXTReader {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            if (e.Args.Length > 0) {
                G.Book = new Book(e.Args[0]);
            }

            if (!checkSuffixName(".trb", "TXTReaderBook", "TXTReader小说", null, AppDomain.CurrentDomain.BaseDirectory + "TXTReader.exe")) {
                if (System.Windows.Forms.MessageBox.Show("你还没有将.trb文件关联到TXTReader，是否设置？", "关联文件", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes) {
                    if (!UACManager.Execute(UACManager.ACTION_REGISTER)) {
                        System.Windows.Forms.MessageBox.Show("设置失败！");
                    }
                }                
            }                
        }

        private static bool checkSuffixName(string ext, string name, string description, string icon, string path) {
            if (IntPtr.Size == 4) {
                var HKCR = Registry.ClassesRoot + "" + "\\";
                if (Registry.GetValue(HKCR + ext, "", "") + "" != name) return false;
                if (description != null) if (Registry.GetValue(HKCR + name, "", "") + "" != description) return false;
                if (icon != null) if (Registry.GetValue(HKCR + name + @"\DefaultIcon", "", "") + "" != icon) return false;
                if (Registry.GetValue(HKCR + name + @"\shell\open\command", "", "") + "" != path + " %1") return false;
            } else if (IntPtr.Size == 8) {
                var HKCR = Registry.ClassesRoot + "";
                if (Registry.GetValue(HKCR + ext, "", "") + "" != name) return false;
                if (description != null) if (Registry.GetValue(HKCR + name, "", "") + "" != description) return false;
                if (icon != null) if (Registry.GetValue(HKCR + name + @"\DefaultIcon", "", "") + "" != icon) return false;
                if (Registry.GetValue(HKCR + name + @"\shell\open\command", "", "") + "" != path + " %1") return false;
            }
            return true;
        }      
    }
}
