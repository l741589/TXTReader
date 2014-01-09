using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Plugins;

using Zlib.Text;

namespace TXTReader.ToolPanel {
    class OptionsParser : XmlParser{
        public const String S_ROOT = "option";
        
        public const String S_SPEED = "speed";
        public const String S_FLOATMESSAGE = "floatmessage";
        public const String S_OPEN = "open";
        public const String S_FPS = "fps";
        public const String S_CHAPTERTITLE = "chaptertitle";
        public const String S_TIME = "time";
        public const String S_PROGRESS = "progress";
        public const String S_BOOK = "book";
        public const String S_ISBORDERED = "isbordered";
        public const String S_FULLSCREEN = "isfullscreen";
        public const String S_LOG = "log";
        public const String S_FILTERSPACE = "filterspace";
        public const String S_PLUGINS = "plugins";


        public static void Save() {
            var w = new Writer(S_ROOT)
                .Write(S_SPEED, G.Options.Speed)
                .Write(S_ISBORDERED, G.Options.IsBordered, new bool[0])
                .Write(S_FULLSCREEN, G.Options.IsFullScreen, new bool[0])
                .Write(S_FILTERSPACE, G.Options.IsFilterSpace, new bool[0]);
            w = w.Start(S_PLUGINS);
            foreach (var e in PluginManager.Instance.Plugins)
                if (e.Value.WriteOption != null)
                    w = w.Start(e.Key).Do(e.Value.WriteOption).End;
            w = w.End;
            w.WriteTo(G.NAME_OPTION);
        }

        public static void Load() {
            var r = new Reader(G.NAME_OPTION)
                .Read(S_SPEED, (n) => { G.Options.Speed = int.Parse(n.InnerText); })
                .Read(S_ISBORDERED, (n) => { G.Options.IsBordered = bool.Parse(n.InnerText); })
                .Read(S_FULLSCREEN, (n) => { G.Options.IsFullScreen = bool.Parse(n.InnerText); })
                .Read(S_FILTERSPACE, (n) => { G.Options.IsFilterSpace = bool.Parse(n.InnerText); });
            r = r.Child(S_PLUGINS);
            foreach (var e in PluginManager.Instance.Plugins)
                if (e.Value.ReadOption != null)
                    r = r.Child(e.Key).Do(e.Value.ReadOption).Parent;
            r = r.Parent;
        }
    }
}
