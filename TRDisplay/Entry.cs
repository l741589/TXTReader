using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TXTReader;
using TXTReader.Plugins;
using Zlib.Text;
using Zlib.Utility;

namespace TRDisplay {
    class Entry : PluginEntry{

        public override string[] Dependency { get { return new String[] { "TXTReader", "*FloatControls" }; } }
        private const String S_SPEED = "speed";
        private const String S_FILTERSPACE = "filterspace";
        
        public override void OnLoad(StartupEventArgs e) {
            G.Timer = new TRTimer2();
            SkinParser.SetDefaultSkin();
            //SkinParser.Load();
            //OptionsParser.Load();
            ReadOption = r => r.Do(new SkinParser())
                .Read(S_SPEED, n => Options.Instance.Speed = int.Parse(n.InnerText))
                .Read(S_FILTERSPACE, n => Options.Instance.IsFilterSpace = bool.Parse(n.InnerText));
            WriteOption = w => {
                return w.Do(new SkinParser())
                    .Write(S_SPEED, Options.Instance.Speed)
                    .Write(S_FILTERSPACE, Options.Instance.IsFilterSpace);
            };
        }

        public override void OnWindowCreate(Window window) {
            var d = new Displayer4();            
            G.MainCanvas.Children.Add(d);            
            AddOptionGroup(new DisplayOptionPanel());
            d.SetBinding(Displayer4.HeightProperty, new Binding("ActualHeight") { Source = G.MainCanvas });
            d.SetBinding(Displayer4.WidthProperty, new Binding("ActualWidth") { Source = G.MainCanvas });
            d.SetBinding(Displayer4.SpeedProperty, "Speed", Options.Instance);

            if (Manager["FloatControls"] != null) {
                Assembly.CreateInstance("TRDisplay.FloatFps");
                Assembly.CreateInstance("TRDisplay.FloatScrollSpeed");
            }
        }

        public override void OnUnload(ExitEventArgs e) {
            //SkinParser.Save();
            //OptionsParser.Save();
        }
    }
}
