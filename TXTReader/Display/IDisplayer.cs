using System;
using System.Windows;
namespace TXTReader.Display {
    public interface IDisplayer{
        void CloseFile();
        int FirstLine { get; set; }
        bool IsPausing { get; set; }
        bool IsScrolling { get; set; }
        double Offset { get; set; }
        void OpenFile(string filename);
        void ReopenFile();
        event ShutdownHandler Shutdown;
        double CanvasHeight { get; }
        double CanvasWidth { get; }
        String[] Text { get; }
        void Update();
    }
}
