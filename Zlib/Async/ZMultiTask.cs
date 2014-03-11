using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zlib.Utility;

namespace Zlib.Async {
    public delegate void ZMultiTaskWorkDoneEventHandler(object sender, ZMultiTaskWorkDoneEventArgs args);
    public class ZMultiTaskWorkDoneEventArgs : EventArgs {
        public object Tag { get; set; }
        public bool Success { get; set; }
        public Action Work { get; set; }
        public Exception Exception { get; set; }
    }
    public class ZMultiTask {

        private List<Tuple<object, Action>> works = new List<Tuple<object, Action>>();
        
        private Stack<Task> tasks = new Stack<Task>();
        private static HashSet<int> ids = new HashSet<int>();

        [ThreadStatic]
        private static int id;
        public static int CurrentId {
            get {
                return id;
            }
        }

        public ZMultiTask() {
        }

        public void Add(Action work,object tag=null) {
            lock (works) { works.Add(Tuple.Create(tag, work)); }
        }       

        public Task Execute(int parallel = 5,bool useThreadPool = true) {
            var t = new Task(this, parallel, works, useThreadPool);
            tasks.Push(t);
            return t;
        }

       

        public async System.Threading.Tasks.Task Shutdown() {
            foreach (var e in tasks) {
                e.Stop();
                if (e.IsRun) await ZEventTask.Wait(e, "AllWorkDone", g => g.Handler = new EventHandler((d, ee) => g.Continue()));
            }
        }

        public class Task {
            public int RunningCount { get; private set; }

            private Thread[] pool;            
            private BaseAwaiter awaiter = null;
            private List<Tuple<object, Action>> works = new List<Tuple<object, Action>>();
            private ZMultiTask owner;
            public bool IsRun { get; set; }
            public event ZMultiTaskWorkDoneEventHandler WorkDone;
            public event EventHandler AllWorkDone;


            internal Task(ZMultiTask owner,int parallel,IEnumerable<Tuple<object, Action>> works,bool useThreadPool) {
                this.owner = owner;
                foreach (var e in works) this.works.Add(e);
                RunningCount = parallel;
                IsRun = true;
                if (useThreadPool) {
                    for (int i = 0; i < parallel; ++i) {
                        ThreadPool.QueueUserWorkItem((e) => DoRun());
                    }
                } else {
                    pool = new Thread[parallel];
                    for (int i = 0; i < parallel; ++i) {
                        pool[i] = new Thread(DoRun);
                        pool[i].Start();
                    }
                }
            }

            //运行在子线程
            private void DoRun() {
                lock (ids) {
                    int i;
                    for (i = 1; ids.Contains(i); ++i) ;
                    id = i;
                    ids.Add(id);
                }
                while (IsRun) {
                    var work = Pop();
                    if (work == null) break;
                    try {
                        work.Item2();
                        OnWorkDone(work);
                    } catch (Exception e) {
                        OnWorkDone(work, e);
                    }
                }
                ThreadDone();
            }

            private void ThreadDone() {
                lock (this) {
                    --RunningCount;
                    if (RunningCount <= 0) {
                        GetAwaiter().DoComplete();
                        IsRun = false;
                        ids.Remove(id);
                    }
                }
            }
       
            public void Stop(){
                IsRun = false;
            }

            private void OnWorkDone(Tuple<object, Action> work, Exception e = null) {
                if (WorkDone != null)
                    if (work == null && e == null) {
                        if (AllWorkDone != null) AllWorkDone(this, EventArgs.Empty);
                    } else {
                        WorkDone(this, new ZMultiTaskWorkDoneEventArgs {
                            Success = e == null,
                            Tag = work.Item1,
                            Work = work.Item2,
                            Exception = e
                        });
                    }
            }

            public void BringToTop(object tag) {
                lock (works) {
                    var o = works.Find(i => i.Item1 == tag);
                    if (o != null) {
                        works.Remove(o);
                        works.Insert(0, o);
                    }
                }
            }

            public void BringIndexToTop(int index) {
                lock (works) {
                    var o = works[index];
                    if (o != null) {
                        works.Remove(o);
                        works.Insert(0, o);
                    }
                }
            }

            public void CircuteIndexToTop(int index) {
                lock (works) {
                    if (works.Count() <= index) return;
                    for (int i = 0; i < index; ++i) {
                        var o = works.First();
                        works.Remove(o);
                        works.Add(o);
                    }
                }
            }

            public void CircuteToTop(object tag) {
                lock (works) {
                    if (works.IsEmpty()) return;
                    if (works.Find(i => i.Item1 == tag) == null) return;
                    while (works.First().Item1 != tag) {
                        var o = works.First();
                        works.Remove(o);
                        works.Add(o);
                    }
                }
            }

            public Action FetchIndex(int index) {
                Action r = null;
                lock (works) {
                    var o = works[index];
                    if (o != null) {
                        works.Remove(o);
                        r = o.Item2;
                    }
                }
                return r;
            }


            public Action Fetch(object tag) {
                Action r = null;
                lock (works) {
                    var o = works.Find(i => i.Item1 == tag);
                    if (o != null) {
                        works.Remove(o);
                        r = o.Item2;
                    }
                }
                return r;
            }

            private Tuple<object, Action> Pop() {
                lock (works) {
                    if (works.Count == 0) return null;
                    var r = works.First();
                    works.RemoveAt(0);
                    return r;
                }
            }

            public BaseAwaiter GetAwaiter() {
                if (awaiter == null) awaiter = new BaseAwaiter();
                return awaiter;
            }

        }        
    }
}