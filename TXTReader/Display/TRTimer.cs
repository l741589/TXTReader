using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows;
using System.Threading;

namespace TXTReader.Display {

    
    class TRTimer : DependencyObject, ITRTimer {

        private Thread timerThread;
        long lastTick = 0;
        public event TRTimerHandler Timer;

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(TRTimer),
            new PropertyMetadata(5, (d, e) => {
                TRTimer t = ((TRTimer)d);
                t.Stop();           
                t.Start();
            }));

        public int Interval { get { return (int)GetValue(IntervalProperty); } set { SetValue(IntervalProperty, value); } }
        public TRTimerStatus Status { get; set; }

        private bool entered;
        private int interval = 0;

        private void Run() {
            Status = TRTimerStatus.RUNNING;
            while (Status == TRTimerStatus.RUNNING) {
                int _interval = interval;
                try {
                    Thread.Sleep(_interval);
                } catch (ThreadInterruptedException) { }
                long curTick = DateTime.Now.Ticks;
                if (lastTick != 0 && !entered) {
                    entered = true;
                    Dispatcher.BeginInvoke(new TRTimerHandler(Work), DispatcherPriority.Send, curTick - lastTick);
                }
                Debug.WriteLine(curTick - lastTick);
                lastTick = curTick;

            }
        }

        private void Work(long tick) {
            OnTRTimer(tick);
            entered = false;            
        }


        void OnTRTimer(long tick) {
            Timer(tick);
        }

        public void Stop() {
            Status = TRTimerStatus.STOPED;
            if (timerThread != null) {
                if (timerThread.IsAlive) {
                    timerThread.Interrupt();
                    timerThread.Join();
                }
                timerThread = null;
            }
        }

        public void Start() {
            interval = Interval;
            if (timerThread == null || !timerThread.IsAlive) {
                timerThread = new Thread(Run);
                lastTick = 0;
                timerThread.Start();
            }
        }

        public void Pause() {
            Status = TRTimerStatus.PAUSED;            
        }

        public void Resume() {
            if (Status == TRTimerStatus.PAUSED)
                Start();
        }
    }
}
