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

namespace TXTReader.Utility {
    static class G {
        static G() {
            Trmex = Trmex.Compile(new String[] { " \"第#卷\" \"第#章*\"", " \"第#卷\" \"[第]#章*\"", " \"外传*\"" });
            Book = new Book();
            NoCover = (ImageSource)new ImageSourceConverter().ConvertFrom(Properties.Resources.URI_NO_COVER);
            Timer = new TRTimer2();
            Bookmark = new ObservableCollection<Bookmark>();
            Books = new BookCollection();
        }
        public static readonly String PATH = AppDomain.CurrentDomain.BaseDirectory;
        public static String PATH_BOOK { get { return A.CheckDir(PATH + @"bookcase\"); } }        
        public static String PATH_BOOKMARK { get { return A.CheckDir(PATH + @"bookmark\"); } }
        public static String EXT_BOOK { get { return ".trb"; } }
        public static String EXT_BOOKMARK { get { return ".trm"; } }
        public static String EXT_OPTION { get { return ".tro"; } }
        public static String EXT_SKIN { get { return ".trs"; } }
        public static ImageSource NoCover { get; private set; }
        public static Trmex Trmex { get; set; }
        public static Book Book { get; set; }
        public static Options Options { get { return Options.Instance; } }
        public static ITRTimer Timer = new TRTimer2();
        public static String FileName { get; set; }
        public static MainWindow MainWindow { get { return App.Current.MainWindow as MainWindow; } }
        public static IDisplayer Displayer { get { return MainWindow.displayer; } }
        public static ObservableCollection<Bookmark> Bookmark { get; private set; }
        public static BookCollection Books { get; private set; }
    }
}
