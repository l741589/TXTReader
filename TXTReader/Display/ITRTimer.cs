using System;
namespace TXTReader.Display {

    delegate void TRTimerHandler(long tick);
    enum TRTimerStatus { STOPED, RUNNING, PAUSED };

    interface ITRTimer {
        int Interval { get; set; }
        void Pause();
        void Resume();
        void Start();
        TRTimerStatus Status { get; set; }
        void Stop();
        event TRTimerHandler Timer;
    }
}
