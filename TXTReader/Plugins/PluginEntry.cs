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

namespace TXTReader.Plugins {
    public abstract class PluginEntry {
        public abstract String[] Dependency { get; }
        public virtual Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        public XmlParserReaderCallback ReadOption { get; set; }
        public XmlParserWriterCallback WriteOption { get; set; }
        public PluginManager Manager { get { return PluginManager.Instance; } }
        public Assembly Assembly { get; set; }
        public abstract void OnLoad(StartupEventArgs e);
        public abstract void OnWindowCreate(Window window);
        public abstract void OnUnload(ExitEventArgs e);

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
    }
}
