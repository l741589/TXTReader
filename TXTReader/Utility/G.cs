using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TXTReader.Data;
using TXTReader.Display;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace TXTReader.Utility {
    static class G {
        static private Book book = null;
        static G() {
            Trmex = Trmex.Compile(new String[] { " \"第#卷\" \"第#章*\"", " \"第#卷\" \"[第]#章*\"", " \"外传*\"" });
            //NoCover = new BitmapImage(new Uri(Properties.Resources.URI_NO_COVER));
            //NoCover = (ImageSource)new ImageSourceConverter().ConvertFrom(Properties.Resources.URI_NO_COVER);
            NoCover = App.Current.Resources["src_nocover"] as ImageSource;
            Timer = new TRTimer2();
            Bookmark = new ObservableCollection<Bookmark>();
            Books = new BookCollection();
            WorkThread = new WorkThread();
            WorkThread.Thread.Start();
        }
        public static String HTTP_HEAD { get { return "http://"; } }
        public static String FILE_HEAD { get { return "file:///"; } }
        public static String PACK_HEAD { get { return "pack://"; } }
        public static String TXTREADER_HEAD { get { return "/TXTReader"; } }

        public static String PATH { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static String PATH_BOOK { get { return A.CheckDir(PATH + @"books\"); } }        
        public static String PATH_COVER { get { return A.CheckDir(PATH + @"cover\"); } }

        public static String EXT_BOOK { get { return ".trb"; } }
        public static String EXT_OPTION { get { return ".tro"; } }
        public static String EXT_SKIN { get { return ".trs"; } }

        public static ImageSource NoCover { get; private set; }
        public static Trmex Trmex { get; set; }
        public static Book Book {
            get { return book; }
            set {
                if (book == value) return;
                if (book != null) book.Close();
                book = value;
                if (book != null) book.Load();
            }
        }
        public static Options Options { get { return Options.Instance; } }
        public static ITRTimer Timer = new TRTimer2();
        public static String FileName { get { return book == null ? null : book.Source; } }
        public static MainWindow MainWindow { get { return App.Current.MainWindow as MainWindow; } }
        public static Displayer4 Displayer { get { return MainWindow.displayer; } }
        public static ObservableCollection<Bookmark> Bookmark { get; private set; }
        public static BookCollection Books { get; private set; }
        public static WorkThread WorkThread { get; private set; }
    }
}
