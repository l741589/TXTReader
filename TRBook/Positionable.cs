using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBook {
    interface Positionable {
        int Position { get; set; }
        double Offset { get; set; }
    }
}
