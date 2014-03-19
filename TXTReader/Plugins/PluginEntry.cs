using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using Zlib.Text;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Input;
using Zlib.Text.Xml;

namespace TXTReader.Plugins {
    public enum PluginState { NotLoad, Ready, NoEntry, Loaded, Fail, NoEnoughNecessaryDependencies, NoEnoughUnnecessaryDependencies }
    public abstract class PluginEntry {
        public abstract String[] Dependency { get; }
        public virtual Version Version { get { return Assembly.GetName().Version; } }
        public virtual String Author { get { return "<匿名>"; } }
        public XmlParserReaderCallback ReadOption { get; set; }
        public XmlParserWriterCallback WriteOption { get; set; }
        public PluginManager Manager { get { return PluginManager.Instance; } }
        public Assembly Assembly { get; set; }
        public abstract void OnLoad(StartupEventArgs e);
        public abstract void OnWindowCreate(Window window);
        public abstract void OnUnload(ExitEventArgs e);
        public PluginState PluginState { get; set; }
        public int Index { get; set; }
        public virtual String Description { get { return "<无描述>"; } }

        protected Dictionary<String, Delegate> APIs = new Dictionary<String, Delegate>();

        public PluginEntry() {
            PluginState = PluginState.NotLoad;
        }

        public void AddToolTab(object header,object item){
            G.ToolTabControl.Items.Add(new TabItem { Header = header, Content = item });
        }

        public void AddContextMenu(params MenuBase[] menu) {
            G.ContextMenu.Add(menu);
        }

        public void AddOptionGroup(object title, UIElement e) {
            GroupBox g = new GroupBox();
            g.Header = title;
            g.Content = e;
            G.OptionPanel.Children.Add(g);
        }

        public void AddOptionGroup(UIElement e) {
            G.OptionPanel.Children.Add(e);
        }

        public override string ToString() {
            return Assembly.GetName().Name;
        }

        public object Execute(String method, params object[] args) {
            if (APIs.ContainsKey(method)) {
                return APIs[method].Method.Invoke(this, args);
            }
            return null;
        }

        public virtual void OnOpen(object arg) { }

        public void RegisterOpenCallback(String ext, String desc) {
            PluginManager.Instance.OpenCallback.Add(ext.Trim(' ', '*', '.'), new Tuple<string, PluginEntry>(desc, this));
        }
    }
}
