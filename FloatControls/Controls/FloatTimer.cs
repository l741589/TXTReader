using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Utility;

namespace FloatControls.Controls {
    class FloatTimer : FloatMessage {

        public FloatTimer()
        {
            Format = "{0:hh:mm:ss}";
            timer();
            Name = "时间";
        }

        private async void timer() {
            while (true) {
                Value = DateTime.Now;
                await 1000;
            }
        }
    }
}
