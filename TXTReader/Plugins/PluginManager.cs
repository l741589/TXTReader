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
        private int _pluginIndex = 0;
        public static PluginManager Instance { get { if (instance == null) instance = new PluginManager(); return instance; } }
        public override string[] Dependency { get { return new String[0]; } }

        public Dictionary<String, PluginEntry> Plugins = new Dictionary<String, PluginEntry>();

        private PluginManager() {
            Index = _pluginIndex++;
            PluginState = PluginState.Loaded;
        }

        public PluginEntry Load(String filename) {
            Assembly a = Assembly.LoadFrom(filename);
            //Assembly a = Assembly.LoadFile(filename);
            var ns = a.GetName().Name;
            PluginEntry entry = a.CreateInstance(ns + ".Entry") as PluginEntry;
            if (entry == null) return null;
            Plugins.Add(ns, entry);
            entry.Assembly = a;
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


        private bool LoadPlugin(PluginEntry e, StartupEventArgs ea) {
            if (e.PluginState != PluginState.NotLoad) {
                if (e.PluginState == PluginState.Fail) return false;
                if (e.PluginState == PluginState.NoEnoughNecessaryDependencies) return false;
                if (e.PluginState == PluginState.Ready) throw new ApplicationException("Cyclic Dependency");
                return true;
            }
            try {
                e.PluginState = PluginState.Ready;
                if (e.Dependency != null) {
                    foreach (var s in e.Dependency) {
                        if (s == "TXTReader") continue;
                        if (s[0] == '*') {
                            var p = this[s.Substring(1)];
                            if (p == null || !LoadPlugin(p, ea)) {
                                e.PluginState = PluginState.NoEnoughUnnecessaryDependencies;
                            }
                        } else {
                            var p = this[s];
                            if (p == null || !LoadPlugin(p, ea)) {
                                e.PluginState = PluginState.NoEnoughNecessaryDependencies;
                                return false;
                            }
                        }
                    }
                }

                Debug.WriteLine("Load Plugin '{0}'", (object)e.Assembly.FullName);
                e.OnLoad(ea);
                if (e.PluginState == PluginState.Ready)
                    e.PluginState = PluginState.Loaded;
                return true;
            } catch(Exception) {
                Debug.WriteLine("Load Plugin '{0}' Fail", (object)e.Assembly.FullName);
                e.PluginState = PluginState.Fail;
                return false;
            } finally {
                e.Index = _pluginIndex++;
            }
        }

        public override void OnLoad(StartupEventArgs e) {
            foreach (var i in Plugins) {
                LoadPlugin(i.Value, e);
                //i.Value.OnLoad(e);
            }
        }

        public override void OnWindowCreate(Window w) {
            foreach (var i in Plugins) i.Value.OnWindowCreate(w);
        }

        public override void OnUnload(ExitEventArgs e) {
            foreach (var i in Plugins) i.Value.OnUnload(e);
        }


        public object Execute(String plugin, String method, params object[] args) {
            var p = this[plugin];
            if (p == null) return null;
            return p.Execute(method, args);
        }
    }
}
