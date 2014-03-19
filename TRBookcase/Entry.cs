using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Interfaces;
using TXTReader.Plugins;
using Zlib.Utility;

namespace TRBookcase {
    class Entry : PluginEntry{
        public override string Author { get { return "大钊"; } }
        public override string[] Dependency { get { return new String[] { "TXTReader","*TRSpider","*FloatControls" }; } }
        public override string Description { get { return "提供书架支持"; } }
        public static String Filename = null;

        public override void OnLoad(System.Windows.StartupEventArgs e) {
            RegisterOpenCallback("trb", "TXTReader小说");
            ReadOption = r => r.Read("book", n => { if (Filename == null) Filename = n.InnerText; });
            WriteOption = w => w.Write("book", Filename);
            if (e.Args != null && e.Args.Length > 0) {
                Filename = e.Args[0];
            }
        }

        public override void OnWindowCreate(System.Windows.Window window) {
            if (PluginManager.Has("FloatControls")) {
                Assembly.CreateInstance("TRBookcase.FloatTiltle");
            }
            BookParser.Load();
            AddToolTab("书架", new BookcasePanel());
            G.BookChanged += (d, e) => {
                if (e.NewBook.NotNull()) {
                    if (e.NewBook is BookCaseItem) return;
                    var b = e.NewBook;
                    var bi = G.Books.GetBook(b);
                    if (bi != null) {
                        bi.Bind(b);
                        bi.AssignTo(b);
                        b = bi;
                    } else {
                        b = new BookCaseItem(b) { IsLoaded = true };
                        BookParser.Save(b);
                        G.Books.Add(b);
                    }                    
                    e.NewBook.Closing += NewBook_Closing;
                    e.NewBook.Loaded += Book_Updated;
                }
            };
            window.Loaded += window_Loaded;
            window.Closing += window_Closing;
            
        }

        void window_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            try {
                if (!String.IsNullOrEmpty(Filename))
                    PluginManager.OpenFile(Filename);
            } catch { }
        }

        void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (G.Book.NotNull()) Filename = G.Book.Source; else Filename = null;
            G.Book = null;
        }

        void NewBook_Closing(object sender, PluginEventArgs e) {
            var b = sender as IBook;
            var bb = b.GetBindingTarget();
            if (bb != null && bb.Count() != 0) b = bb[0] as BookCaseItem ?? b;
            BookParser.Save(b);
            (sender as IBook).Unbind();
        }

        void Book_Updated(object sender, PluginEventArgs e) {
            BookParser.Save(sender as IBook);
        }

        public override void OnUnload(System.Windows.ExitEventArgs e) {
            
        }
    }
}
