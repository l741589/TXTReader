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
using TXTReader.Interfaces;

namespace TRBook {
    class Entry  : PluginEntry {

        public const String S_BOOK = "book";
        public const String S_MINCHAPTERLENGTH = "minchapterlength";
        public const String S_MAXCHAPTERLENGTH = "maxchapterlength";

        public override string[] Dependency { get { return new String[] { "TXTReader", "TRContent", "*FloatControls", "*TRSpider", "*TRBookcase" }; } }

        public override void OnLoad(StartupEventArgs e) {
            Register();
            RegisterOpenCallback("txt", "文本文档");

            ReadOption = r => r
                .Read(S_MINCHAPTERLENGTH, (n) => { Chapter.MinChapterLength = int.Parse(n.InnerText); })
                .Read(S_MAXCHAPTERLENGTH, (n) => { Chapter.MaxChapterLength = int.Parse(n.InnerText); });

            WriteOption = w => w               
                .Write(S_MINCHAPTERLENGTH, Chapter.MinChapterLength)
                .Write(S_MAXCHAPTERLENGTH, Chapter.MaxChapterLength);
        }

        public override void OnWindowCreate(Window window) {
            APIs.Add("open", new Action<String>(s => { Book.Open(s); }));
            AddOptionGroup("目录", new ContentOptionPanel());
            InitCallback(window);
            
            
        }

        private void InitCallback(Window w) {
            w.Loaded += MainWindow_Loaded;
            w.Closing += w_Closing;
            w.KeyDown += w_KeyDown;
        }        

        void MainWindow_Loaded(object sender, RoutedEventArgs e) {            
            
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
           
        }

        public override void OnUnload(ExitEventArgs e) {
            
        }

        private void Register(){
            try {
                if (!RegUtil.CheckSuffixName(".trb", "TXTReaderBook", "TXTReader小说", null, AppDomain.CurrentDomain.BaseDirectory + "TXTReader.exe")) {
                    if (System.Windows.Forms.MessageBox.Show("你还没有将.trb文件关联到TXTReader，是否设置？", "关联文件", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes) {
                        if (!UAC.Execute(CSN)) {
                            System.Windows.Forms.MessageBox.Show("设置失败！\r\n请检查防火墙设置并请确保您拥有管理员权限。");
                        }
                    }
                }
            } catch { }            
        }

        public static void CSN() {
            RegUtil.CreateSuffixName(".trb", "TXTReaderBook", "TXTReader小说", null, AppDomain.CurrentDomain.BaseDirectory + "TXTReader.exe");
        }

        public override string Description { get { return "占用Book插槽\n提供书籍的管理功能"; } }
        public override string Author { get { return "大钊"; } }

        public override void OnOpen(object arg) {
            if (arg is String) {
                Book.Open((String)arg);
            } else if (arg is IBook){
                Book.Open((IBook)arg);
            }
        }
    }
}
