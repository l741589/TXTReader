using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime;

namespace Zlib.Async {
    /*
        public class ZEventTask : ZEventTask<object> {
            public ZEventTask(ResultGetter resultGetter) : base(resultGetter) { }
        }*/



    /*public class ZEventTask_<T> {
        public delegate T ZWaitEventHandler(object sender, ZWaitEventArgs e);
        public delegate void ResultGetter(ZEventTask<T> awaiter);

        private Awaiter<T> awaiter;
        private ResultGetter resultGetter;
        private ZWaitEventHandler handler = null;

        public ZEventTask(ResultGetter resultGetter) {
            this.resultGetter = resultGetter;
            resultGetter(this);
        }

        private void CustomEventHandler(object sender, EventArgs e) {
            if (handler == null) {
                SetResultAndContinue(default(T));
            } else {
                var ee = new ZWaitEventArgs { DoReturn = true, EventArgs = e };
                var r = handler(sender, ee);
                if (ee.DoReturn) SetResultAndContinue(r);
            }
        }

        public static ZEventTask<TT> Wait<H,TT>(object obj, String evt, ZEventTask<TT>.ZWaitEventHandler handler) {
            var e = obj.GetType().GetEvent(evt);
            var r = new ZEventTask<TT>(rg => {                
                e.AddEventHandler(obj, (EventHandler)rg.CustomEventHandler);
            });
            r.handler = handler;
            r.AfterGetResult = (rg) => {
                e.RemoveEventHandler(obj, (EventHandler)rg.CustomEventHandler);
            };
            return r;
        }

        public static ZEventTask Wait(object obj, String evt) {
            var e = obj.GetType().GetEvent(evt);
            var r=new ZEventTask(rg => {                
                e.AddEventHandler(obj, (EventHandler)rg.CustomEventHandler);                
            });
            r.AfterGetResult = (rg) => {
                e.RemoveEventHandler(obj, (EventHandler)rg.CustomEventHandler);
            };
            return r;
        }

        public Awaiter<T> GetAwaiter(){
            return awaiter = new Awaiter<T>();
        }

        public void SetResultAndContinue(T result){
            awaiter.DoComplete(this, result);
        }

        public ResultGetter AfterGetResult;

       

        

        }*/
    public class ZEventTask : ZEventTask<object> {

        public static ZEventTask Wait(object obj, String evt, ZEventTask.ResultGetter getter) {
            var z = new ZEventTask();
            z.Target = obj;
            z.E = obj.GetType().GetEvent(evt);
            if (getter == null) z.Continue(null);
            getter(z);
            if (z.Handler == null) z.Continue(null);
            z.E.AddEventHandler(obj, z.Handler);
            return z;
        }
        
    }
    //public class ZEventTask<HandlerType> : ZEventTask<HandlerType, object> { }

    public class ZEventTask<ResultType> {
        
        public delegate void ResultGetter(ZEventTask<ResultType> e);
       
        protected EventInfo E;
        protected object Target;
        public Delegate Handler;
        protected ResultGetter getter;
        
        
        public static ZEventTask<T> Wait<T>(object obj, String evt, ZEventTask<T>.ResultGetter getter){
            var z = new ZEventTask<T>();
            z.Target = obj;
            z.E = obj.GetType().GetEvent(evt);
            if (getter == null) z.Continue(default(T));
            getter(z);
            if (z.Handler == null) z.Continue(default(T));
            z.E.AddEventHandler(obj, z.Handler);
            return z;
        }

        public void Continue(ResultType result) {
            E.RemoveEventHandler(Target, Handler);
            awaiter.DoComplete(result);
        }

        public void Continue() {
            Continue(default(ResultType));
        }

        protected Awaiter<ResultType> awaiter;
        public Awaiter<ResultType> GetAwaiter() {
            if (awaiter == null) awaiter = new Awaiter<ResultType>();
            return awaiter;
        }
        public class Awaiter<TT> : IAwaiter<TT> {

            private Action continuation;
            private TT result;

            public TT GetResult() { return result; }

            public bool IsCompleted { get; private set; }

            public void OnCompleted(Action continuation) {
                this.continuation = continuation;
            }

            public void DoComplete(TT result) {
                IsCompleted = true;
                this.result = result;
                //if (owner.AfterGetResult != null) owner.AfterGetResult(owner);
                continuation();
            }
        }
    }
}
