using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TXTReader.Utility {
    class TaskCache<T> {
        private LinkedList<Task<T>> tasks=new LinkedList<Task<T>>();
        private Dictionary<Task<T>, AutoResetEvent> map=new Dictionary<Task<T>, AutoResetEvent>();
        
    }
}
