using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zlib.Async;

namespace Zlib.Utility {
    public static class IntExtention {

        public static Task Wait(this int x) {
            return TaskEx.Delay(x);
        }
    }
}
