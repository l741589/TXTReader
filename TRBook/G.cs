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

      
        public static String PATH_COVER { get { return A.CheckDir(PATH + @"cover\"); } }
        public static String PATH_BOOK { get { return A.CheckDir(PATH + @"books\"); } }
        public static String EXT_BOOK { get { return ".trb"; } }
        

        static G() {
            Res = new Res();
            NO_COVER = Res["src_nocover"] as ImageSource;
        }
    }
}
