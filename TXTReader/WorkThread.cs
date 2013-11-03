using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;

namespace TXTReader {
    class WorkThread{

        public Thread Thread { get; set; }
        AutoResetEvent e;
        public bool running;
        ArrayList list = ArrayList.Synchronized(new ArrayList());

        public WorkThread(){
            e = new AutoResetEvent(false);
            Thread = new Thread(Run);
        }

        public void Run() {
            running = true;
            while (running) {
                if (list.Count == 0) e.WaitOne();
                try {
                    ThreadStart t = list[0] as ThreadStart;
                    list.RemoveAt(0);
                    if (t != null) t.Invoke();
                } catch { }
            }
        }

        public void Add<T>(Func<T> proc){
            list.Add(proc);
            e.Set();
        }

        public void Add(ThreadStart proc) {
            list.Add(proc);
            e.Set();
        }

        public void Stop() {
            running = false;
            e.Set();
        }
    }
}
