using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Utility {
    
    public class HttpDelegate : TXTReader.Utility.IHttpDelegate {
        public HttpDelegateEvent AfterRequest { get; set; }
        public HttpDelegateEvent AfterResponse { get; set;}
        public HttpDelegateEvent BeforeRequest { get; set; }
        public HttpDelegateEvent BeforeResponse { get; set; }
    }
}
