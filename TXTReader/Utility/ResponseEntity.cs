using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Utility {
    class ResponseEntity {
        public int status { get; set; }
        public String msg { get; set; }
        public object[] data { get; set; }
    }
}
