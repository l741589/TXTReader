using System;
using System.Collections.Generic;
namespace TXTReader.Interfaces {
    public interface IBook {
        //event EventHandler Loaded;
        event PluginEventHandler LoadFinished;
        event PluginEventHandler Closed;
        event PluginEventHandler PositionChanged;
        event PluginEventHandler OffsetChanged;

        string Author { get; }
        string Id { get; }
        string CurrentTitle { get; }
        string Title { get; }
        List<String> TotalText { get; }
        int Position { get; set; }
        double Offset { get; set; }
        void Close();
        void Open(object arg);
        void Reopen();
        void Delete();
        void Load();
    }
}
