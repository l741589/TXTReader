using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TXTReader.Widget {
    class FloatMessageManager {
        public Canvas Container { get; private set; }
        public List<FloatMessage> FloatMessage { get; private set; }

        public FloatMessageManager(Canvas canvas) {
            Container = canvas;
        }

        public static implicit operator Canvas(FloatMessageManager floatMessageManager) {
            return floatMessageManager.Container;
        }
    }
}
