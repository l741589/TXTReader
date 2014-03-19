using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Zlib.Algorithm;

namespace TRBookcase {
    class G : TXTReader.G{
        public static BookCollection Books = BookCollection.Instance;

        public static readonly Res Res;
        public static readonly String NO_COVER = null;


        public static String PATH_COVER { get { return A.CheckDir(PATH + @"cover\"); } }
        


        static G() {
            Res = new Res();
            NO_COVER = "/TRBookcase;component/res/no_cover.png";
        }
    }
}
