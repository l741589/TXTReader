using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Plugins;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zlib.Utility;
using System.ComponentModel;
using System.Reflection;
using FloatControls;
using Zlib.Win32;

namespace TRBook {
    class Entry  : PluginEntry {

        public static String Filename = null;
        public const String S_BOOK = "book";
        public const String S_MINCHAPTERLENGTH = "minchapterlength";
        public const String S_MAXCHAPTERLENGTH = "maxchapterlength";

        public override string[] Dependency { get { return new String[] { "TXTReader", "TRContent", "*FloatControls", "*TRSpider" }; } }

        public override void OnLoad(StartupEventArgs e) {
            Register();
            BookParser.Load();
            AddContextMenu(G.Res["contextMenu"] as ContextMenu);

            ReadOption = r => r
                .Read(S_BOOK, n => { if (Filename == null) Filename = n.InnerText; })
                .Read(S_MINCHAPTERLENGTH, (n) => { Chapter.MinChapterLength = int.Parse(n.InnerText); })
                .Read(S_MAXCHAPTERLENGTH, (n) => { Chapter.MaxChapterLength = int.Parse(n.InnerText); });

            WriteOption = w => w
                .Write(S_BOOK, Filename)
                .Write(S_MINCHAPTERLENGTH, Chapter.MinChapterLength)
                .Write(S_MAXCHAPTERLENGTH, Chapter.MaxChapterLength);

            if (e.Args != null && e.Args.Length > 0) {
                Filename = e.Args[0];
            }
        }

        public override void OnWindowCreate(Window window) {
            APIs.Add("open", new Action<String>(s => { Book.Open(s); }));
            AddOptionGroup("目录", new ContentOptionPanel());
            InitCallback(window);
            InitCommandBinding(window);
            try {
                if (!String.IsNullOrEmpty(Filename))
                    G.Book= new Book(Filename);
            } catch { }
            if (Manager["FloatControls"] != null) {
                Assembly.CreateInstance("TRBook.FloatTiltle");
                //if (Manager["TRSearchBar"] != null) Assembly.CreateInstance("TRBook.Rules.TrmexComparer");
            }
        }

        private void InitCallback(Window w) {
            w.Loaded += MainWindow_Loaded;
            w.Closing += w_Closing;
            w.KeyDown += w_KeyDown;
        }        

        private void InitCommandBinding(Window w) {
            /*w.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
            (d, e) => {                
                var dlg = new System.Windows.Forms.OpenFileDialog();
                dlg.Filter = Properties.Resources.FILE_FILTER;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) G.Book = new Book(dlg.FileName);
            }, (d, e) => { e.CanExecute = true; }));*/

            w.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
                (d, e) => { 
                    G.Book = null; 
                },
                (d, e) => { e.CanExecute = G.Book.NotNull(); })
            );

            w.CommandBindings.Add(new CommandBinding(MyCommands.Reopen,
                (d, e) => { G.Book.Reopen(); },
                (d, e) => { e.CanExecute = G.Book.NotNull(); })
            );
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            AddToolTab("书签", new BookmarkPanel());
            AddToolTab("书架", new BookcasePanel());
            
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

        void w_Closing(object sender, CancelEventArgs e) {
            if ((G.Book as Book).NotNull()) Filename = (G.Book as Book).Source; else Filename = null;
            G.Book = null;
        }

        public override void OnUnload(ExitEventArgs e) {
            BookParser.Save();
        }

        private void Register(){
            try {
                if (!RegUtil.CheckSuffixName(".trb", "TXTReaderBook", "TXTReader小说", null, AppDomain.CurrentDomain.BaseDirectory + "TXTReader.exe")) {
                    if (System.Windows.Forms.MessageBox.Show("你还没有将.trb文件关联到TXTReader，是否设置？", "关联文件", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes) {
                        if (!UAC.Execute(CSN)) {
                            System.Windows.Forms.MessageBox.Show("设置失败！");
                        }
                    }
                }
            } catch { }            
        }

        public static void CSN() {
            RegUtil.CreateSuffixName(".trb", "TXTReaderBook", "TXTReader小说", null, AppDomain.CurrentDomain.BaseDirectory + "TXTReader.exe");
        }

        public override string Description {
            get {
                return "占用Book插槽\n提供书籍的管理功能";
            }
        }
    }
}
