using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Security.Permissions;
using System.Threading;
using TXTReader.Display;
using TXTReader.ToolPanel;
using TXTReader.Plugins;
using Zlib.Win32;

namespace TXTReader {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {

        public String FileName = null;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            PluginManager.Instance.Load(e);

            
            

            SkinParser.SetDefaultSkin();
            SkinParser.Load();
            OptionsParser.Load();
            if (e.Args.Length > 0) FileName = e.Args[0];

            if (FileName != null) {
                //Book.I = new Book(FileName);
            }
            FileName = null;
        }

        

        protected override void OnExit(ExitEventArgs e) {
            PluginManager.Instance.OnUnload(e);
            SkinParser.Save();
            OptionsParser.Save();
            foreach (EventWaitHandle b in G.Blockers) if (b != null) b.Set();
            base.OnExit(e);
        }
    }
}
