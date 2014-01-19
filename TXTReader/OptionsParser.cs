using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader;
using TXTReader.Plugins;
using Zlib.Text;

namespace TXTReader {
    class OptionsParser : XmlParser{
        public const String S_ROOT = "option";      
        
        private const String S_ISBORDERED = "isbordered";
        private const String S_FULLSCREEN = "isfullscreen";
        private const String S_PLUGINS = "plugins";

        private static bool isBordered;
        public static bool IsBordered {            
            get {
                if (G.MainWindow != null) isBordered = G.MainWindow.IsBordered;
                return isBordered;
            }
            set {
                isBordered = value;
                if (G.MainWindow != null) G.MainWindow.IsBordered = value;                
            }
        }

        private static bool isFullScreen;
        public static bool IsFullScreen{            
            get {
                if (G.MainWindow != null) isFullScreen = G.MainWindow.IsFullScreen;
                return isFullScreen;
            }
            set {
                isFullScreen = value;
                if (G.MainWindow != null) G.MainWindow.IsFullScreen = isFullScreen;                
            }
        }

        public static void Save() {
            var w = new Writer(S_ROOT)
                .Write(S_ISBORDERED, IsBordered)
                .Write(S_FULLSCREEN, IsFullScreen);
            w = w.Start(S_PLUGINS);
            foreach (var e in PluginManager.Instance.Plugins)
                if (e.Value.WriteOption != null)
                    w = w.Start(e.Key).Do(e.Value.WriteOption).End;
            w = w.End;
            w.WriteTo(G.NAME_OPTION);
        }

        public static void Load() {
            var r = new Reader(G.NAME_OPTION)
                .Read(S_ISBORDERED, (n) => { IsBordered = bool.Parse(n.InnerText); })
                .Read(S_FULLSCREEN, (n) => { IsFullScreen = bool.Parse(n.InnerText); });
            r = r.Child(S_PLUGINS);
            foreach (var e in PluginManager.Instance.Plugins)
                if (e.Value.ReadOption != null)
                    r = r.Child(e.Key).Do(e.Value.ReadOption).Parent;
            r = r.Parent;
        }
    }
}
