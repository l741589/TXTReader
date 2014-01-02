using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Net {
    
    public class HttpDelegate : IHttpDelegate {
        public HttpDelegateEvent AfterRequest { get; set; }
        public HttpDelegateEvent AfterResponse { get; set;}
        public HttpDelegateEvent BeforeRequest { get; set; }
        public HttpDelegateEvent BeforeResponse { get; set; }
    }
}
