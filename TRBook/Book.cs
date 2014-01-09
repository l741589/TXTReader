using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TRBook.Net;
using TRBook.Rules;
using TXTReader.Interfaces;
using Zlib.Async;
using Zlib.Utility;

namespace TRBook {
    internal class Book : Chapter, ContentAdapter, Positionable, IBook {
        public enum BookState { Local, Remote, Downloading, Missing };
        public const int DO_GATE = 500;
        public event EventHandler LoadFinished;
        public event EventHandler Loaded;
        public event EventHandler Closed;
        public event EventHandler PositionChanged;
        public event EventHandler OffsetChanged;

        public const String NO_PREVIEW = "暂无预览";
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(int), typeof(Book), new PropertyMetadata(0, OnPositionChanged, PositionCeorce));
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(Book), new PropertyMetadata(0.0, OnOffsetChanged));
        public static readonly DependencyPropertyKey CurrentTitleProperty = DependencyProperty.RegisterReadOnly("CurrentTitle", typeof(String), typeof(Book), new PropertyMetadata(null));
        public static readonly DependencyPropertyKey PreviewProperty = DependencyProperty.RegisterReadOnly("Preview", typeof(String), typeof(Book), new PropertyMetadata(null));
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(BookState), typeof(Book), new PropertyMetadata(BookState.Local));                

        private ImageSource cover = null;
        private String source = null;
        private String id = null;
        private static ZTask uploadTask = new ZTask();
        private static ZTask downloadTask = new ZTask();
        private List<Bookmark> undoList = new List<Bookmark>();
        private int undoPos = 0;

        public ImageSource Cover { get { if (cover.IsNull()) return G.NO_COVER; else return cover; } set { cover = value; } }
        public int Position { get { return (int)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }
        public double Offset { get { return (double)GetValue(OffsetProperty); } set { SetValue(OffsetProperty, value); } }
        public BookState State { get { return (BookState)GetValue(StateProperty); } set { SetValue(StateProperty, value); } }
        public String CurrentTitle { get { return (String)GetValue(CurrentTitleProperty.DependencyProperty); } }
        public String Author { get; set; }
        public DateTime LastLoadTime { get; set; }
        public double SortArgument { get; set; }
        public String Id { get { return id; } set { id = value; Books.Check(this); } }
        public ObservableCollection<Bookmark> Bookmark { get; private set; }
        public bool IsOpen = false;
        public static BookCollection Books { get; private set; }
        
        private static Book book;

        static Book() {
            NullObject.Add(Empty);
            Books = new BookCollection();
        }

        public Book()
            : base() {
            Position = 0; Offset = 0;
            Bookmark = new ObservableCollection<Bookmark>();
            Node = new LinkedListNode<ContentItemAdapter>(this);
        }

        public Book(String src) : this() { Init(src); SortArgument = 0; }
        
        public static Book I {
            get { return book != null ? book : Empty; }
            set {
                if (book == value) return;
                if (book != null) book.Close();
                book = value;
                if (book != null) {
                    book.Load();
                    if (book.Loaded.NotNull()) book.Loaded(I, ExitEventArgs.Empty);
                }
                if (Empty.Loaded.NotNull()) Empty.Loaded(I, ExitEventArgs.Empty);
            }
        }

        private bool init_lock = false;
        public void Init(String src) {
            if (Path.GetExtension(src) == G.EXT_BOOK) {
                BookParser.Load(src, this);
            } else {
                if (init_lock) return;
                init_lock = true;
                init_lock = true;
                Source = src;
                if (Title == null || Title == "") Title = Path.GetFileNameWithoutExtension(src);
                if (File.Exists(BookParser.GetBookPath(this))) BookParser.Load(this);
                LastLoadTime = default(DateTime);
                init_lock = false;
            }
        }


        public String Source {
            get { return source; }
            set {
                source = value;
                if (source == null || source == "") State = BookState.Missing;
                else if (State == BookState.Local) {
                    if (!(source.Contains('\\') || source.Contains('/'))) State = BookState.Remote;
                    else if (!File.Exists(source)) State = BookState.Missing;
                } else {
                    if (File.Exists(source)) State = BookState.Local;
                }
                Books.Check(this);
            }
        }

        public String GetPreview(int position) {
            var t = GetValue(PreviewProperty.DependencyProperty);
            var preview = t != null ? t.ToString() : null;
            preview = GetPreview(preview, position);
            return preview;
        }

        public String GetPreview(String preview, int? position = null) {
            
            if (position == null) position = Position;
            if (TotalText == null || TotalText.Count == 0) {
                if (preview != null) return preview;
                return NO_PREVIEW;
            }
            if (position < 0) return preview;
            if (position >= TotalText.Count) return preview;
            int i = position.Value;
            preview = TotalText[i++];
            while (preview.Length < 256 && i < TotalText.Count)
                preview += "\n" + TotalText[i++];
            return preview;
        }

        public String Preview {
            get {
                var t=GetValue(PreviewProperty.DependencyProperty);
                var preview = t != null ? t.ToString() : null;
                preview = GetPreview(preview);
                SetValue(PreviewProperty, preview);
                return preview;
            }
            set {
                if (value != null && value != "") SetValue(PreviewProperty, value);
            }
        }

        private Chapter Insert(List<String> subtitles, List<int?> numbers, int level, Chapter node) {
            Chapter ret = null;
            if (level > 0) node.Number = numbers[level - 1];
            if (level < subtitles.Count) {
                ret = Insert(subtitles, numbers, level + 1, (Chapter)node[subtitles[level]]);
            } else {
                ret = node;
            }
            return ret;
        }

        public bool Match(ICollection<String> texts) {
            Chapter node = this;
            foreach (var s in texts) {
                if (!IsOpen) return false;
                bool b = true;
                do {
                    if (s.IsNullOrWhiteSpace()) break;
                    Trmex trmex = null;
                    if (Dispatcher.Invoke(() => { return Rule.Instance.IsListEnable; })) trmex = Dispatcher.Invoke(() => { return Rule.Instance.ListTrmex; });
                    ChapterDesc r = trmex.Match(s);
                    if ((r == null || trmex == null) && Dispatcher.Invoke(() => Rule.Instance.IsTreeEnable))
                        trmex = Dispatcher.Invoke(() => Rule.Instance.TreeTrmex);
                    if (r == null) break;
                    if (trmex.LCs == null) {
                        if (node != null) Dispatcher.Invoke(() => { node.Length = 0; var l = node.Length; });
                        node = Dispatcher.Invoke(() => { return Insert(r.SubTitle, r.Numbers, 0, this); });
                    } else {
                        var n = node;
                        while (n.Children != null) n = n.Children.Last.Value as Chapter;
                        while (n.Level < r.Level - 1) n = n["未命名章节"];
                        while (n.Level > r.Level - 1 && n != null) n = n.Parent as Chapter;
                        if (node != null) Dispatcher.Invoke(() => { node.Length = 0; var l = node.Length; });
                        node = n[r.Title];
                        node.Number = r.Numbers[node.Level - 1];
                    }
                    b = false;
                } while (false);
                if (b) node.AppendText(s);
            }
            return true;
        }


        public async void Load(String file = null) {
            if (file == null) file = Source; else Source = file;
            if (State == BookState.Local) {
                Clear();
                var ss = File.ReadAllLines(file, Encoding.Default);
                Title = Path.GetFileNameWithoutExtension(file);
                if (Text != null) Text.Clear();
                if (File.Exists(Source)) {
                    TotalText = ss.ToList();
                    BookParser.Load(this);
                    LastLoadTime = DateTime.Now;
                    IsOpen = true;
                    bool matchresult = await Task<bool>.Run(() => Match(TotalText));
                    TotalText = null;
                    if (matchresult) Update();
                } else {
                    State = BookState.Missing;
                    Update();
                }                
                GenerateIndex();
                TotalText = null;
                Dispatcher.Invoke(() => { 
                    if (LoadFinished != null) LoadFinished(this,EventArgs.Empty);
                    if (Empty.LoadFinished != null) Empty.LoadFinished(this, EventArgs.Empty);
                });
            } else {
                await Download();
                if (State == BookState.Local) Load(Source);
            }
        }

        public async Task Download() {
            //BookState state = State;
            //await downloadTask.Run(() => {
            //    if (state == BookState.Remote) {
            //        Dispatcher.Invoke(() => { State = BookState.Downloading; });
            //        string id = Id;
            //        ResponseEntity s = G.Net.Download(id);
            //        if (s.status != MyHttp.successCode) {
            //            Debug.WriteLine(s.msg);
            //            Dispatcher.Invoke(() => { State = BookState.Remote; });
            //            Dispatcher.BeginInvoke(new Action(() => { G.Log = Title + " 下载失败"; }));
            //            return;
            //        }
            //        Debug.WriteLine(s.data[0]);
            //        Debug.WriteLine(Encoding.Default.GetString((byte[])s.data[1]));
            //        String path = G.PATH_SOURCE + Path.GetFileNameWithoutExtension(s.data[0].ToString()) + "_" + id + Path.GetExtension(s.data[0].ToString());
            //        File.WriteAllBytes(path, (byte[])s.data[1]);
            //        Dispatcher.Invoke(() => {
            //            Source = path;
            //            Title = Path.GetFileNameWithoutExtension(s.data[0].ToString());
            //            BookParser.Save(this);
            //            Dispatcher.BeginInvoke(new Action(() => { G.Log = Title + " 下载完成"; }));
            //        });
            //    }
            //});
        }

        public async Task Upload() {

            //await uploadTask.Run(() => {
            //    String title = Dispatcher.Invoke(() => { return Title; });
            //    Debug.WriteLine("Start Upload " + title);
            //    String src = Dispatcher.Invoke(() => { return Source; });
            //    if (BookState.Local == Dispatcher.Invoke(() => { return State; })) {
            //        ResponseEntity res = G.Net.Upload(A.CheckExt(title, ".txt"), src);
            //        if (res.status == MyHttp.successCode) {
            //            Debug.WriteLine(title + " Uploaded");
            //            Dispatcher.Invoke(() => {
            //                Id = res.data[0].ToString();
            //                BookParser.Save(this);
            //            });
            //            Dispatcher.BeginInvoke(new Action(() => { G.Log = Title + " 上传完成"; }));
            //        } else {
            //            Debug.WriteLine(title + " Upload Error:" + res.msg);
            //            Dispatcher.BeginInvoke(new Action(() => { G.Log = Title + " 上传失败:" + G.Net[res.status][1]; }));
            //        }
            //    }
            //});

        }

        public String ToolTip {
            get {
                String ret = "";
                if (Title != null) ret += Title + "\n";
                if (Author != null) ret += "作者：" + Author + "\n";
                if (Length != 0) ret += "长度：" + Length + "字\n";
                if (Source != "" && Source != null) ret += Source + "\n";
                if (Id != null && Id != "") ret += Id + "\n";
                //if (Preview != null) ret += "内容：\n" + Preview + "\n";
                return ret.Trim();
            }
        }

        public void MoreInfo() {
            Douban.MoreInfo(this);
        }

        public override void Close() {
            if (this.IsNull()) return;
            var p = Preview;
            BookParser.Save(this);
            LoadFinished = null;
            IsOpen = false;
            if (Closed != null) Closed(this, EventArgs.Empty);
            if (Empty.Closed != null) Empty.Closed(this, EventArgs.Empty);            
            base.Close();
            book = null;
        }

        private List<ContentItemAdapter> positions;
        public ContentItemAdapter[] Positions;
        private void GenerateIndex() {
            positions = new List<ContentItemAdapter>();
            GenerateIndex(this);
            Positions = positions.ToArray();
        }

        private void GenerateIndex(ContentItemAdapter node) {
            if (node.Children != null && node.Children.Count > 0) {
                foreach (var e in node.Children) GenerateIndex(e);
            } else {
                positions.Add(node);
            }
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d.IsNull()) return;
            var o = d as Book;
            if (Math.Abs((int)e.OldValue - (int)e.NewValue) > DO_GATE) o.DoDo();
            if (o.Positions != null) {
                int i = (int)e.NewValue;
                int l = 0, r = o.Positions.Length;
                int m = (l + r) >> 1;
                while (true) {
                    var p = o.Positions[m];
                    if (i < p.AbsolutePosition) {
                        r = m;
                    } else if (i >= p.AbsolutePosition) {
                        if (m == r - 1) break;
                        var q = o.Positions[m + 1];
                        if (i < q.AbsolutePosition) {
                            break;
                        } else {
                            l = m;
                        }
                    }
                    m = (l + r) >> 1;
                    if (m == 0) break;
                }
                o.SetValue(Book.CurrentTitleProperty, o.Positions[m].TotalTitle);
            } else {
                o.SetValue(Book.CurrentTitleProperty, o.TotalTitle);
            }
            if (o.PositionChanged.NotNull()) o.PositionChanged(o, EventArgs.Empty);
            if (Empty.PositionChanged.NotNull()) Empty.PositionChanged(o, EventArgs.Empty);
            
        }

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
            if (d.IsNull()) return;
            var o = d as Book;
            if (o.OffsetChanged.NotNull()) o.OffsetChanged(o,EventArgs.Empty);
            if (Empty.OffsetChanged.NotNull())Empty.OffsetChanged(o,EventArgs.Empty);
        }

        private static object PositionCeorce(DependencyObject d, object baseValue) {
            int x = (int)baseValue;
            Book b = d as Book;
            if (x >= b.TotalLineCount) {
                b.Offset = 0;
                x = b.TotalLineCount - 1;
            }
            if (x < 0) {
                b.Offset = 0;
                x = 0;
            }
            return x;
        }


        private bool dodoLock = false;
        public bool CanUndo { get { return undoPos > 1 && !dodoLock; } }

        public void Undo() {
            Debug.WriteLine("UNDO:" + CanUndo);
            if (!CanUndo) return;
            dodoLock = true;
            --undoPos;
            undoList[undoPos - 1].AssignTo(this);            
            //G.Displayer.Update();
            dodoLock = false;
        }

        public bool CanRedo { get { return undoPos < undoList.Count && !dodoLock; } }

        public void Redo() {
           Debug.WriteLine("REDO:" + CanRedo);
            if (!CanRedo) return;
            dodoLock = true;            
            ++undoPos;
            undoList[undoPos - 1].AssignTo(this);
            //G.Displayer.Update();
            dodoLock = false;
        }

        public void DoDo() {
            if (this.IsNull()) return;
            if (dodoLock) return;
            dodoLock = true;
            Debug.WriteLine("DODO");
            Bookmark bmk = new Bookmark(this);
            while (undoList.Count > undoPos) undoList.RemoveAt(undoList.Count - 1);
            undoList.Add(bmk);
            undoPos = undoList.Count;
            dodoLock = false;
        }

        public static Book Empty = new Book() { Bookmark = null };

        public static void Reopen(){
            if (I.IsNull()) return;
            String f=I.source;
            I=null;
            I = new Book(f);
        }

        public static void Open(String filename) {
            if (I.Source == filename) return;
            I = new Book(filename);
        }

        void IBook.Open(string filename) { Book.Open(filename); }

        void IBook.Reopen() { Book.Reopen(); }

        public void Delete() {
            if (!Source.IsNullOrEmpty())
                File.Delete(Source);
        }

        public override void Update() {
            base.Update();
            var p = Preview;
            BookParser.Save(this);
        }
    }
}
