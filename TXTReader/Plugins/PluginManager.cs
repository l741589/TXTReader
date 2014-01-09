using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zlib.Utility;

namespace TXTReader.Plugins {
    public class PluginManager : PluginEntry{
        private static PluginManager instance;
        public static PluginManager Instance { get { if (instance == null) instance = new PluginManager(); return instance; } }

        public Dictionary<String, PluginEntry> Plugins = new Dictionary<String, PluginEntry>();

        public PluginEntry Load(String filename) {
            Assembly a = Assembly.LoadFrom(filename);
            //Assembly a = Assembly.LoadFile(filename);
            var ns = a.GetName().Name;
            PluginEntry entry = a.CreateInstance(ns + ".Entry") as PluginEntry;
            if (entry == null) return null;
            Plugins.Add(ns, entry);
            entry.Assembly = a;
            Debug.WriteLine("Load Plugin:'{0}'", (object)ns);
            return entry;
        }

        public PluginEntry this[String name] {
            get {
                if (Plugins.ContainsKey(name)) return Plugins[name];
                else return null;
            }
        }

        public void Load(StartupEventArgs e = null) {
            var d=Directory.CreateDirectory(G.PATH_PLUGINS);
            //var d = Directory.CreateDirectory(G.PATH);
            var fis = d.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
            foreach(var fi in fis) Load(fi.FullName);
            OnLoad(e);
        }

        public override void OnLoad(StartupEventArgs e) {
            foreach (var i in Plugins) i.Value.OnLoad(e);
        }

        public override void OnWindowCreate(Window w) {
            foreach (var i in Plugins) i.Value.OnWindowCreate(w);
        }

        public override void OnUnload(ExitEventArgs e) {
            foreach (var i in Plugins) i.Value.OnUnload(e);
        }


       
    }
}
