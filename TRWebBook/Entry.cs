using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TRSpider;
using TXTReader;
using TXTReader.Plugins;
using Zlib.Utility;

namespace TRWebBook {
    class Entry  : PluginEntry{
        private static Res Res = new Res();
        public static readonly ImageSource NO_COVER = null;
        static Entry() {
            NO_COVER = Res["src_nocover"] as ImageSource;
        }

        public override string[] Dependency {
            get { return new String[] { "TXTReader", "TRSpider", "*FloatControls" }; }
        }

        public override string Author { get { return "大钊"; } }

        public override string Description {
            get {
                return "提供在线看小说的支持";
            }
        }

        public override void OnLoad(System.Windows.StartupEventArgs e) {
            AddContextMenu(Res["menu"] as ContextMenu);
            APIs.Add("+downloadinfo", new Func<BookDownloader, object>(d => {
                if (Manager["FloatControls"] != null) {
                    var t = Type.GetType("TRWebBook.DownloadInfo");
                    if (t == null) return null;
                    var c = t.GetConstructor(new Type[] { typeof(BookDownloader) });
                    if (c == null) return null;
                    return c.Invoke(new object[] { d });

                }
                return null;
            }));
            APIs.Add("-downloadinfo", new Action<object>(o => {
                var m = o.GetType().GetMethod("Stop");
                if (m == null) return;
                m.Invoke(o, null);
            }));
        }

        public override void OnWindowCreate(System.Windows.Window w) {
            w.CommandBindings.Add(Res["searchBinding"] as CommandBinding);
            w.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
               (d, e) => { G.Book = null; },
               (d, e) => { e.CanExecute = G.Book.NotNull(); })
           );

            w.CommandBindings.Add(new CommandBinding(MyCommands.Reopen,
                (d, e) => { G.Book.Reopen(); },
                (d, e) => { e.CanExecute = G.Book.NotNull(); })
            );

            w.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
            (d, e) => {
                var dlg = new System.Windows.Forms.OpenFileDialog();
                dlg.Filter = "可编辑书籍|*.trbx";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) G.Book = new Book(dlg.FileName);
            }, (d, e) => { e.CanExecute = true; }));
            w.KeyDown+=w_KeyDown;
        }

        void w_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.System) {
                switch (e.SystemKey) {
                    case Key.Left: if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
                            if ((G.Book as Book).NotNull()) (G.Book as Book).Undo();
                        break;
                    case Key.Right: if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
                            if ((G.Book as Book).NotNull()) (G.Book as Book).Redo();
                        break;
                }
            }
        }

        public override void OnUnload(System.Windows.ExitEventArgs e) {
            
        }

        
    }
}
