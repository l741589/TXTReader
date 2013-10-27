using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Data {
    interface Positionable {
        int Position { get; set; }
        double Offset { get; set; }
    }
}
