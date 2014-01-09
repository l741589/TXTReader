using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace Zlib.Win32 {
    public static  class UAC {


        /// <summary>
        /// 申请管理员权限，并在管理员权限下执行一个函数
        /// </summary>
        /// <param name="action">被执行的函数，该函数必须为公开静态函数（public static）,且不依赖上下文。</param>
        /// <returns>执行结果，正常完成返回true，否则返回false</returns>
        public static bool Execute(Action action) {
            Process p = new Process();
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.Arguments = String.Join(" ", action.Method.DeclaringType.Assembly.Location, action.Method.DeclaringType.FullName, action.Method.Name);
            startinfo.WorkingDirectory = Environment.CurrentDirectory;
            startinfo.FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"/ZlibUAC.exe";
            startinfo.Verb = "runas";
            p.StartInfo = startinfo;
            try {
                if (!p.Start()) return false;
                p.WaitForExit();
                return 0 == p.ExitCode;
            } catch {
                return false;
            }
        }
    }
}
