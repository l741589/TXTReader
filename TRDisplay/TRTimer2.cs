using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TXTReader.Interfaces;

namespace TRDisplay {
    class TRTimer2 : DependencyObject, ITRTimer {

        long lastTick = 0;
        public event TRTimerHandler Timer;

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(TRTimer2),
            new PropertyMetadata(5, (d, e) => {
                TRTimer2 t = ((TRTimer2)d);
                t.Stop();
                t.Start();
            }));

        public int Interval { get { return (int)GetValue(IntervalProperty); } set { SetValue(IntervalProperty, value); } }
        public TRTimerStatus Status { get; set; }

        private void Work(long tick) {
            OnTRTimer(tick);
        }


        void OnTRTimer(long tick) {
            Timer(tick);
        }

        public void Stop() {
            Status = TRTimerStatus.STOPED;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            lastTick = 0;
        }

        public void Start() {
            Status = TRTimerStatus.RUNNING;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public void Pause() {
            if (Status == TRTimerStatus.RUNNING) {
                Stop();
                Status = TRTimerStatus.PAUSED;
            }
        }

        public void Resume() {
            if (Status == TRTimerStatus.PAUSED)
                Start();
        }

        void CompositionTarget_Rendering(object sender, EventArgs e) {
            if (lastTick == 0) {
                lastTick = DateTime.Now.Ticks;
            } else {
                long t = DateTime.Now.Ticks;
                OnTRTimer(t-lastTick);
                lastTick = t;
            }
        }
    }
}
