using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Zlib.Win32 {
    public static class RegUtil {

        public static bool CheckSuffixName(string ext, string name, string description, string icon, string path) {
            if (IntPtr.Size == 4) {
                var HKCR = Registry.ClassesRoot + "" + "\\";
                if (Registry.GetValue(HKCR + ext, "", "") + "" != name) return false;
                if (description != null) if (Registry.GetValue(HKCR + name, "", "") + "" != description) return false;
                if (icon != null) if (Registry.GetValue(HKCR + name + @"\DefaultIcon", "", "") + "" != icon) return false;
                if (Registry.GetValue(HKCR + name + @"\shell\open\command", "", "") + "" != path + " %1") return false;
            } else if (IntPtr.Size == 8) {
                var HKCR = Registry.ClassesRoot + "" + "\\";
                if (Registry.GetValue(HKCR + ext, "", "") + "" != name) return false;
                if (description != null) if (Registry.GetValue(HKCR + name, "", "") + "" != description) return false;
                if (icon != null) if (Registry.GetValue(HKCR + name + @"\DefaultIcon", "", "") + "" != icon) return false;
                if (Registry.GetValue(HKCR + name + @"\shell\open\command", "", "") + "" != path + " %1") return false;
            }
            return true;
        }

        public static void CreateSuffixName(string ext, string name, string description, string icon, string path) {
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
