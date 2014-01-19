using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Plugins;
using System.Diagnostics;
using TRBook.Rules;
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

        public override string[] Dependency { get { return new String[] { "TXTReader", "*FloatControls", "*TRSearchBar" }; } }

        public override void OnLoad(StartupEventArgs e) {
            G.EmptyBook = Book.Empty;
            Register();
            RuleParser.Load();
            BookParser.Load();
            AddContextMenu(G.Res["contextMenu"] as ContextMenu);

            ReadOption = r => r
                .Read(S_BOOK, n => Filename = n.InnerText)
                .Read(S_MINCHAPTERLENGTH, (n) => { Chapter.MinChapterLength = int.Parse(n.InnerText); })
                .Read(S_MAXCHAPTERLENGTH, (n) => { Chapter.MaxChapterLength = int.Parse(n.InnerText); });

            WriteOption = w => w
                .Write(S_BOOK, Filename)
                .Write(S_MINCHAPTERLENGTH, Chapter.MinChapterLength)
                .Write(S_MAXCHAPTERLENGTH, Chapter.MaxChapterLength);
        }

        public override void OnWindowCreate(Window window) {
            AddOptionGroup("目录", new ContentOptionPanel());
            InitCallback(window);
            InitCommandBinding(window);
            if (!String.IsNullOrEmpty(Filename))
                Book.I = new Book(Filename);
            if (Manager["FloatControls"] != null) {
                Assembly.CreateInstance("TRBook.FloatTiltle");
                if (Manager["TRSearchBar"] != null) Assembly.CreateInstance("TRBook.Rules.TrmexComparer");
            }
        }

        private void InitCallback(Window w) {
            w.Loaded += MainWindow_Loaded;
            w.Closing += w_Closing;
            w.KeyDown += w_KeyDown;
        }        

        private void InitCommandBinding(Window w) {
            w.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
            (d, e) => {                
                var dlg = new System.Windows.Forms.OpenFileDialog();
                dlg.Filter = Properties.Resources.FILE_FILTER;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) Book.I = new Book(dlg.FileName);
            }, (d, e) => { e.CanExecute = true; }));

            w.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
                (d, e) => { Book.I = null; },
                (d, e) => { e.CanExecute = Book.I != Book.Empty; })
            );

            w.CommandBindings.Add(new CommandBinding(MyCommands.Reopen,
                (d, e) => { Book.Reopen(); },
                (d, e) => { e.CanExecute = Book.I != Book.Empty; })
            );
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            AddToolTab("书签", new BookmarkPanel());
            AddToolTab("目录", new ContentTreePanel());
            AddToolTab("书架", new BookcasePanel());
            AddToolTab("规则", new RulePanel());
        }

        void w_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.System) {
                switch (e.SystemKey) {
                    case Key.Left: if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
                            if (Book.I != Book.Empty) Book.I.Undo();
                        break;
                    case Key.Right: if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
                            if (Book.I != Book.Empty) Book.I.Redo();
                        break;
                }
            } 
        }

        void w_Closing(object sender, CancelEventArgs e) {
            Filename = Book.I.Source;
            Book.I = null;
        }

        public override void OnUnload(ExitEventArgs e) {
            BookParser.Save();
            RuleParser.Save();
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
    }
}
