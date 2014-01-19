using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TXTReader.Plugins;
using System.Windows.Data;
using FloatControls.Controls;
using Zlib.Converter;

namespace FloatControls {
    class Entry : PluginEntry {

        public const String S_OPEN="show";

        public static void Add(IFloatControl control) {
            G.FloatControls.Add(control);
        }

        public override void OnLoad(StartupEventArgs e) {

            ReadOption = r => r.Do(n => G.FloatControls.Show = n.Attributes[S_OPEN] == null ? true : bool.Parse(n.Attributes[S_OPEN].Value))
                .ForChildren(null, n => G.FloatControls[n.Name] = bool.Parse(n.InnerText) ? Visibility.Visible : Visibility.Collapsed);

            WriteOption = w => {
                w = w.Attr(S_OPEN, G.FloatControls.Show);
                foreach (IFloatControl c in G.FloatControls)
                    if (c.Tag!=null)
                        w = w.Write(c.Tag.ToString(), G.FloatControls[c.Tag.ToString()] == Visibility.Visible);
                return w;
            };
        }

        public override void OnWindowCreate(Window window) {
            G.FloatControls.Add(new FloatTimer());
            G.FloatControlsPanel = FloatControlsPanel.Instance;            
            G.MainCanvas.Children.Add(FloatControlsPanel.Instance);
            G.FloatControlsPanel.SetBinding(FloatControlsPanel.WidthProperty, new Binding("ActualWidth") { Source = G.MainCanvas });
            G.FloatControlsPanel.SetBinding(FloatControlsPanel.HeightProperty, new Binding("ActualHeight") { Source = G.MainCanvas });
            AddOptionGroup(new FloatControlOptionPanel());
        }

        public override void OnUnload(ExitEventArgs e) {
            
        }

        public override string[] Dependency {
            get {
                return new String[] { "TXTReader" };
            }
        }
    }
}
