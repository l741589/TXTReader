using System;
using System.Windows;
using System.Windows.Controls;
namespace TXTReader.Interfaces {
    public interface IDisplayer{
        int FirstLine { get; set; }
        double Offset { get; set; }
        bool IsPausing { get; set; }
        bool IsScrolling { get; set; }        
        String[] Text { get; }
        void Update();
        ContextMenu ContextMenu { get; set; }
        void LineModify(double n);
        void PageModify(double n);
        void UpdateSkin();
    }
}
