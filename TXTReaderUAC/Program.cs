using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Utility;
using Microsoft.Win32;

namespace TXTReaderUAC {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 1) return;
            switch (args[0]) {
                case TXTReader.Utility.UACManager.ACTION_REGISTER: createSuffixName(".trb", "TXTReaderBook", "TXTReader小说", null, AppDomain.CurrentDomain.BaseDirectory+"TXTReader.exe"); break;
                default: break;
            }
        }

        private static void createSuffixName(string ext, string name, string description, string icon, string path) {
            if (IntPtr.Size == 4) {
                RegistryKey reg = Registry.ClassesRoot;
                reg.CreateSubKey(ext).SetValue("", name);
                if (description != null) reg.CreateSubKey(name).SetValue("", description);
                if (icon != null) reg.CreateSubKey(name + @"\DefaultIcon").SetValue("", icon);
                reg.CreateSubKey(name + @"\shell\open\command").SetValue("", path + " %1");
            } else if (IntPtr.Size == 8) {
                RegistryKey reg = Registry.ClassesRoot;
                reg.CreateSubKey(ext).SetValue("", name);
                if (description != null) reg.CreateSubKey(name).SetValue("", description);
                if (icon != null) reg.CreateSubKey(name + @"\DefaultIcon").SetValue("", icon);
                reg.CreateSubKey(name + @"\shell\open\command").SetValue("", path + " %1");
            }
        }
    }
}
