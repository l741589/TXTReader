using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using TXTReader;
using TXTReader.Plugins;
using Zlib.Text;
using Zlib.Text.Xml;
using Zlib.Utility;

namespace TRDisplay {
    class Entry : PluginEntry{

        public override string[] Dependency { get { return new String[] { "TXTReader", "*FloatControls" }; } }
        private const String S_SPEED = "speed";
        private const String S_FILTERSPACE = "filterspace";
        private IXmlParsable FcSkinParser = null;
        
        public override void OnLoad(StartupEventArgs e) {
            G.Timer = new TRTimer2();
            SkinParser.SetDefaultSkin();
            //SkinParser.Load();
            //OptionsParser.Load();
            if (Manager["FloatControls"] != null) {
                FcSkinParser = (IXmlParsable)Manager["FloatControls"].Assembly.CreateInstance("FloatControls.SkinParser");
            }
            ReadOption = r => r.Do(new SkinParser()).Child("FloatControls").Do(FcSkinParser).Parent
                .Read(S_SPEED, n => Options.Instance.Speed = int.Parse(n.InnerText))
                .Read(S_FILTERSPACE, n => Options.Instance.IsFilterSpace = bool.Parse(n.InnerText));
            WriteOption = w => {
                return w.Do(new SkinParser()).Start("FloatControls").Do(FcSkinParser).End
                    .Write(S_SPEED, Options.Instance.Speed)
                    .Write(S_FILTERSPACE, Options.Instance.IsFilterSpace);
            };
        }

        public override void OnWindowCreate(Window window) {
            var d = new Displayer4();            
            G.MainCanvas.Children.Add(d);            
            d.SetBinding(Displayer4.HeightProperty, new Binding("ActualHeight") { Source = G.MainCanvas });
            d.SetBinding(Displayer4.WidthProperty, new Binding("ActualWidth") { Source = G.MainCanvas });
            d.SetBinding(Displayer4.SpeedProperty, "Speed", Options.Instance);
            AddOptionGroup(new DisplayOptionPanel());
            if (Manager["FloatControls"] != null) {
                Assembly.CreateInstance("TRDisplay.FloatFps");
                Assembly.CreateInstance("TRDisplay.FloatScrollSpeed");
                
            }
            window.KeyDown += window_KeyDown;
        }

        void window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            switch (e.Key) {
                case Key.OemComma: --Options.Instance.Speed; break;
                case Key.OemPeriod: ++Options.Instance.Speed; break;
            }
        }

        public override void OnUnload(ExitEventArgs e) {
            //SkinParser.Save();
            //OptionsParser.Save();
        }

        public override string Description {
            get {
                return "提供显示内容的界面";
            }
        }
    }
}
