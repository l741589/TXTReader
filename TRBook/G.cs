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
        public static readonly String NO_COVER = null;

        static G() {
            Res = new Res();
            NO_COVER = "/TRBook;component/res/no_cover.png";
        }
    }
}
