using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Zlib.Algorithm;
using Zlib.Utility;

namespace Zlib.Text.ZMatchExpression {
    class MatcherCapture {
        public Piece[] Pieces = null;
        public MatcherCapture Parent = null;
        public String Group = null;
        public int[] Numbers;
        public int Index { get; set; }
        public int Length { get { return Text == null ? 0 : Text.Length; } }
        public String Text { get; set; }
        public override string ToString() { return Text; }
    }
}
