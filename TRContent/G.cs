using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zlib.Algorithm;

namespace TRContent {
    class G : TXTReader.G{
        public static readonly Res Res;

        public static String PATH_RULE { get { return A.CheckDir(PATH + @"rules\"); } }
        public static String PATH_LISTRULE { get { return A.CheckDir(PATH + @"rules\"); } }
        public static String PATH_TREERULE { get { return A.CheckDir(PATH + @"rules\"); } }
        public static String PATH_RULEOPTION { get { return A.CheckDir(PATH + @"rules\"); } }

       
        public static String EXT_RULE { get { return ".trr"; } }
        public static String EXT_LISTRULE { get { return ".trml"; } }
        public static String EXT_TREERULE { get { return ".trmt"; } }
        public static String EXT_RULEOPTION { get { return ".trmo"; } }

        static G() {
            Res = new Res();
        }
    }
}
