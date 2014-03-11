using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Threading;
using TXTReader.ToolPanel;
using Zlib.Win32;
using TXTReader.Net;
using Zlib.Async;
using Zlib.Algorithm;
using Zlib.UI;
using TXTReader.Interfaces;
using TXTReader.Plugins;
using Zlib.Utility;
using Zlib;
using System.Windows.Input;

namespace TXTReader {
    public class BookChangedEventArgs : EventArgs {
        public IBook OldBook { get; set; }
        public IBook NewBook { get; set; }
        public BookChangedEventArgs() { }
        public BookChangedEventArgs(IBook OldBook, IBook NewBook) {
            this.OldBook = OldBook;
            this.NewBook = NewBook;
        }
    }
    public delegate void BookChangedEventHandler(object sender,BookChangedEventArgs e);

    public class G  {

        static G() {
            KeyHook = new KeyHook();
            Net = new MyHttp(Properties.Settings.Default.SERVER_URL);
            ContextMenu = new CompoundContextMenu();
            ContextMenu.IsMutexCommand = true;
            CommandManager = new ObjectMutexManager<RoutedUICommand>();
        }
        public static bool IsRunning = true;

        #region const
        public static String TXTREADER_HEAD { get { return "/TXTReader"; } }
        public static String PATH { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static String EXT_OPTION { get { return ".tro"; } }
        public static String EXT_SKIN { get { return ".trs"; } }
        public static String NAME_SKIN { get { return PATH + "skin" + EXT_SKIN; } }
        public static String NAME_OPTION { get { return PATH + "option" + EXT_OPTION; } }
        public static String PATH_PLUGINS { get { return A.CheckDir(PATH + @"plugins\"); } }
        public static String PATH_SOURCE { get { return A.CheckDir(PATH + @"source\"); } }
        #endregion


        public static MainWindow MainWindow { get; internal set; }
        public static TabControl ToolTabControl { get { return MainWindow.toolPanel.tab; } }
        public static StackPanel OptionPanel { get { return MainWindow.toolPanel.pn_option.body; } }
        public static CompoundContextMenu ContextMenu { get; private set; }
        public static Canvas MainCanvas { get { return MainWindow.canvas; } }

        private static IBook book;
        public static IBook Book {
            get { return book; }
            set {
                if (book == value) return;
                if (book != null) book.Close();
                book = value;
                if (book != null) book.Load();
                var old = book; book = value;
                if (BookChanged != null) BookChanged(book, new BookChangedEventArgs(old, book));
            }
        }
        public static event BookChangedEventHandler BookChanged;
        //public static IBook EmptyBook { get; set; }

        public static IDisplayer Displayer { get; set; }
        public static TRNotifyIcon NotifyIcon { get; set; }
        public static KeyHook KeyHook { get; private set; }
        public static List<EventWaitHandle> Blockers { get { return ZTask.Blockers; } }
        public static ITRTimer Timer { get; set; }
        public static MyHttp Net { get; private set; }
        public static ObjectMutexManager<RoutedUICommand> CommandManager { get; private set; }
    }
}
