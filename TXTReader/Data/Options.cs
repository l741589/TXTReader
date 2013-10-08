using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Data {
    class Options {
        public Skin Skin { get; set; }
        public int Speed { get; set; }
        public FloatMessage FloatMessage { get; set; }
        public bool IsFloatMessageOpen { get; set; }
        public int MaxChapterLength { get; set; }
        public int MinChapterLength { get; set; }
    }
}
