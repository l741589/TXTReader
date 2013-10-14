using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Data;

namespace TXTReader.Utility {
    class G {
        static G() {
            Trmex = Trmex.Compile(new String[] { " \"第#卷\" \"第#章*\"", " \"第#卷\" \"[第]#章*\"", " \"外传*\"" });
            RootChapter = new RootChapter();
        }

        public static Trmex Trmex { get; set; }
        public static RootChapter RootChapter { get; set; }
    }
}
