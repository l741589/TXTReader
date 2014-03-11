using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TXTReader.Plugins;
using TXTReader;
using System.Windows.Controls;

namespace TRSearchBar {
    class Entry : PluginEntry {
        public override string[] Dependency { get { return new String[] { "TXTReader" }; } }

        public override void OnLoad(StartupEventArgs e) {
            AddContextMenu(Res.Instance["menu"] as ContextMenu);
        }

        public override void OnWindowCreate(Window window) {
            var p = new SearchBar();
            G.MainCanvas.Children.Add(p);
        }

        public override void OnUnload(ExitEventArgs e) {
        }

        public override string Description {
            get {
                return "提供搜索框功能";
            }
        }
    }
}
