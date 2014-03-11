using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    public class ZSSException : Exception {
        public ZSSException() : base() { }
        public ZSSException(String message) : base(message) { }       
    }

    public class ZSSParseException : Exception {
        public ZSSParseException () : base() { }
        public ZSSParseException(String message) : base(message) { }
        public ZSSParseException(String message,Exception inner) : base(message,inner) { }
    }

    public class ZSSRuntimeException : Exception {
        public ZSSRuntimeException() : base() { }
        public ZSSRuntimeException(String message) : base(message) { }
        public ZSSRuntimeException(String message, Exception inner) : base(message, inner) { }
    }
}
