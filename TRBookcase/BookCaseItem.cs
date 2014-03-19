using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TXTReader.Interfaces;
using TXTReader.Plugins;
using Zlib.Algorithm;
using Zlib.Net;
using Zlib.Utility;
using System.Threading;
using System.Diagnostics;

namespace TRBookcase {
    public class BookCaseItem : IBook, INotifyPropertyChanged {

        public bool IsLoaded = false;

        public BookCaseItem() {
            Data = new Dictionary<string, object>();
            Cover = G.NO_COVER;
        }

        public BookCaseItem(IBook b) {
            Data = new Dictionary<string, object>();
            AssignFrom(b);
            this.Bind(b);
        }

        public BookCaseItem(String source) {
            Cover = G.NO_COVER;
            Author = null;
            Id = null;
            Title = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(source));
            Position = 0;
            Offset = 0;
            Preview = "暂无预览";
            Source = source;
            LastLoadTime = DateTime.Now;
            SortArgument = 0;
            Data = new Dictionary<string, object>();
        }

        public event PluginEventHandler Loaded;
        public event PluginEventHandler Closed;
        public event PluginEventHandler Closing;
        public event PluginEventHandler PositionChanged;
        public event PluginEventHandler OffsetChanged;
        private IBook Target = null;
        public String Cover { get { return _Cover; } set { DownloadCover(this, value); } } private String _Cover;
        public String Author { get { return _Author; } set { _Author = value; OnPropertyChanged("Author"); } } private String _Author;
        public String Id { get { return _Id; } set { _Id = value; OnPropertyChanged("Id"); } } private String _Id;
        public String Title { get { return _Title; } set { _Title = value; OnPropertyChanged("Title"); } } private String _Title;
        public int Position { get { return _Position; } set { _Position = value; OnPropertyChanged("Position"); } } private int _Position;
        public double Offset { get { return _Offset; } set { _Offset = value; OnPropertyChanged("Offset"); } } private double _Offset;
        public String Preview { get { return _Preview; } set { _Preview = value; OnPropertyChanged("Preview"); } } private String _Preview;
        public String Source { get { return _Source; } set { _Source = value; OnPropertyChanged("Source"); } } private String _Source;
        public DateTime LastLoadTime { get { return _LastLoadTime; } set { _LastLoadTime = value; OnPropertyChanged("LastLoadTime"); } } private DateTime _LastLoadTime;
        public double SortArgument { get { return _SortArgument; } set { _SortArgument = value; OnPropertyChanged("SortArgument"); } } private double _SortArgument;
        public string CurrentTitle { get { return null; } }
        public List<string> TotalText { get { return null; } }
        public void Close() { }
        public void Open(object arg) { }
        public void Reopen() { }
        public void Delete() {
            String p = BookParser.GetBookPath(this);
            if (File.Exists(p)) {
                File.Delete(p);
            }
        }

        public Dictionary<String, object> Data { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String prop) {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
            //BookParser.Save(this);
#if DEBUG
            if (prop!="ToolTip") GenToolTip();
#endif
        }

        private static async void DownloadCover(BookCaseItem book, String src) {
            if (book == null) return;
            do {
                if (src.IsNullOrWhiteSpace()) break;
                if (src == G.NO_COVER) break;
                if (src.StartsWith(A.HTTP_HEAD)) {
                    Debug.WriteLine("try enter");
                    ZWeb.Lock();
                    Debug.WriteLine("entered");
                    byte[] bs = null;
                    try { bs = await ZWeb.Instance.DownloadDataTaskAsync(src); } catch { bs = null; }
                    ZWeb.ReleaseLock();
                    Debug.WriteLine("left");
                    if (bs == null) {
                        src = G.NO_COVER;
                    } else {
                        src = G.PATH_COVER + A.MD5(bs) + Path.GetExtension(src);
                        if (!File.Exists(src)) File.WriteAllBytes(src, bs);
                    }
                }
                if (!File.Exists(src))
                    src = G.NO_COVER;
            } while (false);
            book._Cover = src;
            BookParser.Save(book);
            book.OnPropertyChanged("Cover");
        }

        public void AssignFrom(IBook book) {
            Title = book.Title;
            Source = book.Source;
            LastLoadTime = book.LastLoadTime;
            Cover = book.Cover;
            Author = book.Author;
            Id = book.Id;
            Position = book.Position;
            Offset = book.Offset;
            Preview = book.Preview;
        }

        public void AssignTo(IBook book) {
            book.Title = this.Title;
            book.Source = this.Source;
            book.LastLoadTime = this.LastLoadTime;
            book.Cover = this.Cover;
            book.Author = this.Author;
            book.Position = this.Position;
            book.Offset = this.Offset;
            book.Preview = this.Preview;
        }

        public void Load() {
            String f = Source;
            String ext = System.IO.Path.GetExtension(f).Trim('*', ' ', '.').ToLower();
            if (PluginManager.Instance.OpenCallback.ContainsKey(ext)) {
                if (G.Book.IsNull() || G.Book.Source != Source) {
                    PluginManager.Instance.OpenCallback[ext].Item2.OnOpen(this);
                    if (G.Book != null) G.Book.Bind(this);
                }
            }
        }

        public object ToolTip { get { return _ToolTip; } set { _ToolTip = value; OnPropertyChanged("ToolTip"); } } private object _ToolTip;      
        private void GenToolTip(){
            var val = Position + " / " + Offset;
            //if (val!=(String)ToolTip) Debug.WriteLine("TP UPDATE: " + ToolTip);
            ToolTip = val;
            

        }

    }
}
