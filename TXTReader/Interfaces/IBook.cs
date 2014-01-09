using System;
using System.Collections.Generic;
namespace TXTReader.Interfaces {
    public interface IBook {
        event EventHandler Loaded;
        event EventHandler LoadFinished;
        event EventHandler Closed;
        event EventHandler PositionChanged;
        event EventHandler OffsetChanged;

        string Author { get; }
        string Id { get; }
        string CurrentTitle { get; }
        string Title { get; }
        List<String> TotalText { get; }
        int Position { get; set; }
        double Offset { get; set; }
        void Close();
        void Open(String filename);
        void Reopen();
        void Delete();
    }
}
