using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.Reflection;

namespace ZlibUAC {
    class Program {
        static int Main(string[] args) {
            try {
                if (args.Length < 1) return -1;
                var a = Assembly.LoadFrom(args[0]);
                var c = a.GetType(args[1]);
                var m = c.GetMethod(args[2]);
                m.Invoke(null, null);
                return 0;
            } catch(Exception e) {
                return e.HResult;
            }
            //Console.ReadKey();
        }
    }
}
