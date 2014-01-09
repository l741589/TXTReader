using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TXTReader.Display;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using TXTReader.Widget;
using System.Threading;
using TXTReader.ToolPanel;
using Zlib.Win32;
using TXTReader.Net;
using Zlib.Async;
using Zlib.Algorithm;
using Zlib.Widget;
using TXTReader.Interfaces;

namespace TXTReader {
    public class G  {

        static G() {
            Timer = new TRTimer2();
            KeyHook = new KeyHook();
            Net = new MyHttp(Properties.Settings.Default.SERVER_URL);
            ContextMenu = new CompoundContextMenu();
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
        #endregion

        public static Options Options { get { return Options.Instance; } }

        public static MainWindow MainWindow { get { return App.Current.MainWindow as MainWindow; } }
        public static TabControl ToolTabControl { get { return MainWindow.toolPanel.tab; } }
        public static StackPanel OptionPanel { get { return MainWindow.toolPanel.pn_option.body; } }
        public static CompoundContextMenu ContextMenu { get; private set; }
        public static Canvas MainCanvas { get { return MainWindow.canvas; } }

        public static IBook Book { get; set; }
        public static IBook EmptyBook { get; set; }

        public static ITRTimer Timer = new TRTimer2();
        public static Displayer4 Displayer { get { return MainWindow.displayer; } }
        public static TRNotifyIcon NotifyIcon { get; set; }
        public static KeyHook KeyHook { get; private set; }
    
        public static List<EventWaitHandle> Blockers { get { return ZTask.Blockers; } }

        public static MyHttp Net { get; private set; }
    }
}
