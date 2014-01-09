using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace FloatControls {
    class G : TXTReader.G {
        public static FloatControlsPanel FloatControlsPanel { get; set; }
        public static FloatControlOptionPanel FloatControlOptionPanel { get; set; }
        public static FloatControlCollection FloatControls = new FloatControlCollection();
    }
}
