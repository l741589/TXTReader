using System;
using System.Windows;
namespace TXTReader.Display {
    public interface IDisplayer{
        int FirstLine { get; set; }
        bool IsPausing { get; set; }
        bool IsScrolling { get; set; }
        double Offset { get; set; }
        event RoutedEventHandler Shutdown;
        double CanvasHeight { get; }
        double CanvasWidth { get; }
        String[] Text { get; }
        void Update();
    }
}
