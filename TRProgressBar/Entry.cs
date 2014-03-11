using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TXTReader.Plugins;

namespace TRProgressBar {
    class Entry : PluginEntry{

        public override void OnLoad(StartupEventArgs e) {
            
        }

        public override void OnWindowCreate(Window window) {
            if (Manager[Dependency[1]] != null) {
                Assembly.CreateInstance("TRProgressBar.ProgressBar");
            }
        }

        public override void OnUnload(ExitEventArgs e) {
            
        }

        public override string[] Dependency {
            get {
                return new String[] { "TXTReader", "FloatControls", "TRBook" };
            }
        }

        public override string Description {
            get {
                return "提供进度条功能";
            }
        }
    }
}
