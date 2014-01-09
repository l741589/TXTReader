using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Zlib.Algorithm;
using Zlib.Utility;
using System.Windows;
using System.Windows.Markup;
using System.IO;
using System.Windows.Resources;
using System.Windows.Media.Imaging;

namespace TRBook {
    internal class G : TXTReader.G {
        public static readonly Res Res;
        public static readonly ImageSource NO_COVER = null;

        public static String PATH_SOURCE { get { return A.CheckDir(PATH + @"source\"); } }

        public static String PATH_COVER { get { return A.CheckDir(PATH + @"cover\"); } }
        public static String PATH_BOOK { get { return A.CheckDir(PATH + @"books\"); } }
        public static String PATH_RULE { get { return A.CheckDir(PATH + @"rules\"); } }
        public static String PATH_LISTRULE { get { return A.CheckDir(PATH + @"rules\"); } }
        public static String PATH_TREERULE { get { return A.CheckDir(PATH + @"rules\"); } }
        public static String PATH_RULEOPTION { get { return A.CheckDir(PATH + @"rules\"); } }

        public static String EXT_BOOK { get { return ".trb"; } }
        public static String EXT_RULE { get { return ".trr"; } }
        public static String EXT_LISTRULE { get { return ".trml"; } }
        public static String EXT_TREERULE { get { return ".trmt"; } }
        public static String EXT_RULEOPTION { get { return ".trmo"; } }

        static G() {
            Res = new Res();
            NO_COVER = Res["src_nocover"] as ImageSource;
            EmptyBook = TRBook.Book.Empty;
        }
    }
}
