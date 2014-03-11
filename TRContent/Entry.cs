using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using TRContent.Rules;
using TXTReader.Interfaces;
using TXTReader.Plugins;

namespace TRContent {
    class Entry : PluginEntry {

        public override string[] Dependency {
            get { return new String[] { "TXTReader", "*TRSearchBar", "*FloatControls" }; }
        }

        public override string Description {
            get {
                return "提供目录支持,及章节划分算法，并提供相应的API";
            }
        }
        public override void OnLoad(StartupEventArgs e) {
            RuleParser.Load();
        }

        public override void OnWindowCreate(Window window) {
            APIs.Add("update", new Action<IContentAdapter>(b => { ContentTreePanel.Instance.UpdateContentUI(b); }));
            APIs.Add("+SelectChange", new Action<ContentSelectedItemChangedEventHandler>(b => { ContentTreePanel.Instance.SelectedItemChanged += b; }));
            APIs.Add("-SelectChange", new Action<ContentSelectedItemChangedEventHandler>(b => { ContentTreePanel.Instance.SelectedItemChanged -= b; }));
            AddToolTab("目录", ContentTreePanel.Instance);
            AddToolTab("规则", new RulePanel());
            if (Manager["FloatControls"] != null) {
                //Assembly.CreateInstance("TRBook.FloatTiltle");
                if (Manager["TRSearchBar"] != null) Assembly.CreateInstance("TRBook.Rules.TrmexComparer");
            }
        }

        public override void OnUnload(ExitEventArgs e) {
            RuleParser.Save();
        }
    }
}
