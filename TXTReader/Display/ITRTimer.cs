using System;
namespace TXTReader.Display {

    public delegate void TRTimerHandler(long tick);
    public enum TRTimerStatus { STOPED, RUNNING, PAUSED };

    public interface ITRTimer {
        int Interval { get; set; }
        void Pause();
        void Resume();
        void Start();
        TRTimerStatus Status { get; set; }
        void Stop();
        event TRTimerHandler Timer;
    }
}
