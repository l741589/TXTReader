using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zlib.Text.ZMatchExpression {
    class Piece {
        public String Text;
        public int Index;
        public int Length { get { return Text != null ? Text.Length : 0; } }

        public override string ToString() { return Text != null ? Text : ""; }
    }
}
