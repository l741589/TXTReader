using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TXTReader.Utility {
    public static  class UACManager {
        public const String ACTION_REGISTER = "Register";
        public static bool Execute(String action) {
            Process p = new Process();
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.Arguments = action;
            startinfo.WorkingDirectory = Environment.CurrentDirectory;
            startinfo.FileName = "TXTReaderUAC.exe";
            startinfo.Verb = "runas";
            p.StartInfo = startinfo;
            try {
                return p.Start();
            } catch{
                return false;
            }
        }
    }
}
