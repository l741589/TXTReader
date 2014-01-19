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
using TXTReader.ToolPanel;
using TXTReader.Plugins;
using Zlib.Win32;

namespace TXTReader {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {


        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            PluginManager.Instance.Load(e);
            OptionsParser.Load();
        }       

        protected override void OnExit(ExitEventArgs e) {
            OptionsParser.Save();
            PluginManager.Instance.OnUnload(e);
            foreach (EventWaitHandle b in G.Blockers) if (b != null) b.Set();
            base.OnExit(e);
        }
    }
}
