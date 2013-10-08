using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Data {
    class Options {

        static private Options instance;
        static public Options Instance {
            get {
                if (instance == null) instance = new Options();
                return instance;
            }
        }
        private Skin skin;
        public Skin Skin { get { if (skin == null) skin = new Skin(); return skin; } set { skin = value; } }
        public int Speed { get; set; }
        public FloatMessage FloatMessage { get; set; }
        public bool IsFloatMessageOpen { get; set; }
        public int MaxChapterLength { get; set; }
        public int MinChapterLength { get; set; }
    }
}
