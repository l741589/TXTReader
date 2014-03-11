using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Zlib.Async {

#if NET45
    public class TaskEx : Task { }
#endif

    public class BaseAwaiter : BaseAwaiter<object>,IAwaiter {
        public void DoComplete(Exception e = null) {
            DoComplete(null, e);
        }
    }
    public class BaseAwaiter<T> : IAwaiter<T> {

        protected static SynchronizationContext _UIContext = SynchronizationContext.Current;
        protected T Value;
        protected Action continuation;
        protected Exception E = null;
        public Action Work { get; set; }
        public bool IsCompleted { get; private set; }

        public virtual void OnCompleted(Action continuation) {
            this.continuation = continuation;
            if (_UIContext == null) _UIContext = SynchronizationContext.Current;
            if (_UIContext == null) {
                MessageBox.Show("No UI Context");
                return;
            }
            IsCompleted = false;
            if (Work != null) Work();
        }

        public virtual T GetResult() {
            if (E != null) throw E;
            return Value;
        }

        public void DoComplete(T result, Exception e = null) {
            if (_UIContext==null || continuation == null) return;
            Value = result;
            E = e;
            IsCompleted = true;
            _UIContext.Send((t) => {
                continuation();
            }, null);
        }
    }
}
