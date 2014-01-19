using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FloatControls;

namespace TRDisplay {
    class FloatScrollSpeed : FloatMessage {
        public FloatScrollSpeed() {
            Format = "[{0}]";
            Name = "滚屏速度";
            SetBinding(ValueProperty, new Binding("Speed") { Source=TXTReader.G.Displayer });
            this.Register();
        }
    }
}
