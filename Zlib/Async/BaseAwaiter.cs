using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zlib.Async {

    public abstract class BaseAwaiter : BaseAwaiter<object> { }
    public abstract class BaseAwaiter<T> : IAwaiter<T> {

        protected static SynchronizationContext _UIContext = SynchronizationContext.Current;
        protected T Value;
        protected Exception E = null;

        protected abstract T Work();
        public bool IsCompleted { get; private set; }

        public void OnCompleted(Action continuation) {
            IsCompleted = false;
            ThreadPool.QueueUserWorkItem((state) => {
                try {
                    Value = Work();
                } catch (Exception e) {
                    E = e;
                }
                IsCompleted = true;
                _UIContext.Send(new SendOrPostCallback((target) => { continuation(); }), null);
            });
        }

        public T GetResult() {
            if (E != null) throw E;
            return Value;
        }
    }
}
