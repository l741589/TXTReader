using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
namespace TXTReader.Interfaces {
    public interface IBook: INotifyPropertyChanged {
        //event EventHandler Loaded;
        event PluginEventHandler Loaded;
        event PluginEventHandler Closed;
        event PluginEventHandler Closing;
        event PluginEventHandler PositionChanged;
        event PluginEventHandler OffsetChanged;
        String Cover { get; set; }
        string Author { get; set; }
        string Id { get; }
        string CurrentTitle { get; }
        string Title { get; set; }
        List<String> TotalText { get; }
        int Position { get; set; }
        double Offset { get; set; }
        String Preview { get; set; }
        String Source { get; set; }
        DateTime LastLoadTime { get; set; }
        double SortArgument { get; set; }
        void Close();
        void Open(object arg);
        void Reopen();
        void Delete();
        void Load();
    }
}
