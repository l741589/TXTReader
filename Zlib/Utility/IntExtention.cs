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

        public static IntAwaiter GetAwaiter(this int x){
            return new IntAwaiter(x);
        }

        public class IntAwaiter : BaseAwaiter<int> {
            private int time = 0;
            public IntAwaiter(int x){
                time = x;
            }

            protected override int Work() {
                if (time > 0) Thread.Sleep(time);
                return time;
            }
        }
    }
}
