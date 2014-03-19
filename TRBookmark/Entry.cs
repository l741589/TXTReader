using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBookcase;
using TXTReader.Plugins;
using Zlib.Utility;

namespace TRBookmark {
    class Entry : PluginEntry {
        public override string Author { get { return "大钊"; } }
        public override string[] Dependency { get { return new String[] { "TXTReader", "TRContent", "TRBookcase" }; } }
        public override string Description { get { return "提供书签支持"; } }

        private BookmarkPanel panel;

        public override void OnLoad(System.Windows.StartupEventArgs e) {
            BookParser.Extend.Add(new BookmarkParser());
        }

        public override void OnWindowCreate(System.Windows.Window window) {
            panel = new BookmarkPanel();
            window.Loaded += window_Loaded;
            TXTReader.G.BookChanged += G_BookChanged;
        }

        void G_BookChanged(object sender, TXTReader.BookChangedEventArgs e) {
            if (e.NewBook != null) {
                e.NewBook.Closed += NewBook_Closed;
                e.NewBook.Loaded += NewBook_Loaded;
            }            
        }

        void NewBook_Loaded(object sender, TXTReader.Interfaces.PluginEventArgs e) {
            if (!e.Args.ContainsKey("state") || e.Args["state"].ToString() == "init" || e.Args["state"].ToString() == "all") {
                var ss=TXTReader.G.Book.GetBindingSource();
                if (ss != null && ss.Count() > 0) {
                    var s = ss[0] as BookCaseItem;
                    if (!s.Data.ContainsKey("bookmark")) s.Data["bookmark"] = new ObservableCollection<Bookmark>();
                    panel.lb_bookmark.ItemsSource = s.Data["bookmark"] as IEnumerable;
                }
            }
        }

        void NewBook_Closed(object sender, TXTReader.Interfaces.PluginEventArgs e) {
            panel.lb_bookmark.ItemsSource = null;
        }

        void window_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            AddToolTab("书签", panel);
        }

        public override void OnUnload(System.Windows.ExitEventArgs e) {

        }
    }
}
