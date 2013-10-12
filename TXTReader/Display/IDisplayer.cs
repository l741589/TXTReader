using System;
using System.Windows;
namespace TXTReader.Display {
    public interface IDisplayer{
        void CloseFile();
        string FileName { get; set; }
        int FirstLine { get; set; }
        bool IsPausing { get; set; }
        bool IsScrolling { get; set; }
        double Offset { get; set; }
        void OpenFile(string filename);
        void ReopenFile();
        event ShutdownHandler Shutdown;
        string[] Text { get; set; }
        double CanvasHeight { get; }
        double CanvasWidth { get; }
    }
}
