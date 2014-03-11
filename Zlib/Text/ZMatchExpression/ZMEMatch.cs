using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zlib.Text.ZMatchExpression {
    public class ZMECapture {
        public String Text { get; internal set; }
        public int[] Numbers { get; internal set; }
        public override string ToString() { return Text; }
    }

    public class ZMEMatch {
        public ZMECapture[] Captures { get; internal set; }
        public String Text { get; internal set; }
        public int Position { get; internal set; }
        public override string ToString() { return Text; }
        public int Depth { get; internal set; }
    }
}
