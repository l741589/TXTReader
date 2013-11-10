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
using TXTReader.Widget;

namespace TXTReader.Utility {
    static class G {
        static private Book book = null;
        static private Trmex treeTrmex = null;
        static private Trmex listTrmex = null;
        static G() {
            //Trmex = new String[] { " \"第#卷\" \"第#章*\"", " \"第#卷\" \"[第]#章*\"", " \"外传*\"" };
            NoCover = App.Current.Resources["src_nocover"] as ImageSource;
            Timer = new TRTimer2();
            Books = new BookCollection();
            Rules = new Rules();
        }
        public static String HTTP_HEAD { get { return "http://"; } }
        public static String FILE_HEAD { get { return "file:///"; } }
        public static String PACK_HEAD { get { return "pack://"; } }
        public static String TXTREADER_HEAD { get { return "/TXTReader"; } }

        public static String PATH { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static String PATH_BOOK { get { return A.CheckDir(PATH + @"books\"); } }        
        public static String PATH_COVER { get { return A.CheckDir(PATH + @"cover\"); } }
        public static String PATH_RULE  { get { return A.CheckDir(PATH + @"rules\"); } }
        public static String PATH_LISTRULE { get { return A.CheckDir(PATH + @"rules\"); } }
        public static String PATH_TREERULE { get { return A.CheckDir(PATH + @"rules\"); } }
        public static String PATH_RULEOPTION { get { return A.CheckDir(PATH + @"rules\"); } }

        public static String EXT_BOOK { get { return ".trb"; } }
        public static String EXT_OPTION { get { return ".tro"; } }
        public static String EXT_SKIN { get { return ".trs"; } }
        public static String EXT_RULE { get { return ".trr"; } }
        public static String EXT_LISTRULE { get { return ".trml"; } }
        public static String EXT_TREERULE { get { return ".trmt"; } }
        public static String EXT_RULEOPTION { get { return ".trmo"; } }

      
        public static ImageSource NoCover { get; private set; }
        public static Book Book {
            get { return book; }
            set {
                if (book == value) return; 
                if (book != null) book.Close(); 
                book = value; 
                if (book != null) book.Load();
                MainWindow.toolPanel.pn_bookmark.lb_bookmark.ItemsSource = G.Bookmark;
            }
        }
        public static Options Options { get { return Options.Instance; } }
        public static ITRTimer Timer = new TRTimer2();
        public static String FileName { get { return book == null ? null : book.Source; } }
        public static MainWindow MainWindow { get { return App.Current.MainWindow as MainWindow; } }
        public static Displayer4 Displayer { get { return MainWindow.displayer; } }
 
        public static ObservableCollection<Bookmark> Bookmark { get { return Book == null ? null : Book.Bookmark; } }
        public static BookCollection Books { get; private set; }

        public static Trmex ListTrmex { get { if (listTrmex != null) return listTrmex; else  return listTrmex = new Trmex(Rules.CurrentList); } set { listTrmex = value; } }
        public static Trmex TreeTrmex { get { if (treeTrmex != null) return treeTrmex; else  return treeTrmex = new Trmex(Rules.CurrentTree); } set { treeTrmex = value; } }
        public static Rules Rules { get; private set; }
    }
}
