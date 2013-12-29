using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Concurrent;

namespace TXTReader.Utility {

    public class ZTask {
        private Queue<Task> calls = new Queue<Task>();
        public AutoResetEvent are = new AutoResetEvent(true);

        public ZTask(int count = 1) {
            while (count-- > 0) {
                new Thread(DoRun).Start();                
            }
            G.Blockers.Add(are);
        }

        public void Add(Task task) {
            lock (calls) { calls.Enqueue(task); }
            are.Set();
        }

        public Task Pop() {
            lock (calls) {
                if (calls.Count == 0) return null;
                return calls.Dequeue();
            }
        }

        public void DoRun() {
            while (G.IsRunning) {
                var work = Pop();
                while (work != null && G.IsRunning) {
                    try {
                        var tmp = work.Call.DynamicInvoke();
                        work.GetAwaiter().DoComplete(tmp);
                        work = Pop();
                    } catch (Exception e) {
                        #if DEBUG
                        Debug.WriteLine(e.StackTrace);                       
                        throw;
                        #endif
                    }
                }
                are.WaitOne();
            }
        }

        public Task Run<T>(Func<T> call) {
            return new Task(this, call);
        }

        public Task Run(Action call) {
            return new Task(this, new Func<object>(() => { call(); return null; }));
        }


        public class Task {
            public Delegate Call { get; set; }
            public ZTask context;

            public Task(ZTask context,Delegate call) {
                this.context = context;
                this.Call = call;
            }

            private ZTaskAwaiter awaiter;
            public ZTaskAwaiter GetAwaiter() {
                return awaiter != null ? awaiter : awaiter = new ZTaskAwaiter(this);
            }
        }

        public class ZTaskAwaiter  : INotifyCompletion{
            private SynchronizationContext context;
            public bool IsCompleted { get; private set; }
            private Action continuation;
            private object result;
            private Task task;

            public ZTaskAwaiter(Task task) {
                this.context = SynchronizationContext.Current;
                this.task = task;
            }

            public bool BeginAwait(Action continuation) {
                OnCompleted(continuation);
                return true;
            }

            public object EndAwait()
            {
                return GetResult();
            }

            public void OnCompleted(Action continuation) {
                IsCompleted = false;
                this.continuation = continuation;
                task.context.Add(task);
            }

            public void DoComplete(object result) {
                this.result = result;
                IsCompleted = true;
                context.Send((t) => { continuation(); }, null);
            }

            public object GetResult() {
                return result;
            }
        }
    }
}
