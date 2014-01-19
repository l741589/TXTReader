using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FloatControls;

namespace TRDisplay {
    class FloatFps : FloatMessage {
        public FloatFps() {
            Format = "{0}FPS";
            Name = "帧率";
            SetBinding(ValueProperty, new Binding("Fps") { Source=TXTReader.G.Displayer });
            this.Register();
        }
    }
}
