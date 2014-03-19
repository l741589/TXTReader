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
using TXTReader.Interfaces;
using TXTReader.Net;
using TXTReader.Plugins;
using Zlib.Algorithm;
using Zlib.Async;
using Zlib.Text.ZMatchExpression;
using Zlib.Utility;
using TRContent;
using TRContent.Rules;
using System.ComponentModel;

namespace TRBook {
    internal class Book : Chapter, IContentAdapter, IPositionable, IBook {
        public enum BookState { Local, Remote, Downloading, Missing };
        public const int DO_GATE = 500;
        public event PluginEventHandler Loaded;
        public event PluginEventHandler Closed;
        public event PluginEventHandler Closing;
        public event PluginEventHandler PositionChanged;
        public event PluginEventHandler OffsetChanged;

        public const String NO_PREVIEW = "暂无预览";
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(int), typeof(Book), new PropertyMetadata(0, OnPositionChanged, PositionCeorce));
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(Book), new PropertyMetadata(0.0, OnOffsetChanged));
        public static readonly DependencyPropertyKey CurrentTitleProperty = DependencyProperty.RegisterReadOnly("CurrentTitle", typeof(String), typeof(Book), new PropertyMetadata(null));
        public static readonly DependencyPropertyKey PreviewProperty = DependencyProperty.RegisterReadOnly("Preview", typeof(String), typeof(Book), new PropertyMetadata(null));
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(BookState), typeof(Book), new PropertyMetadata(BookState.Local));

        private String cover = null;
        private String source = null;
        private String id = null;
        private static ZTask uploadTask = new ZTask();
        private static ZTask downloadTask = new ZTask();
        private List<Positionable> undoList = new List<Positionable>();
        private int undoPos = 0;
        private bool IsLoaded { get; set; }

        public String Cover { get { if (cover.IsNull()) return G.NO_COVER; else return cover; } set { cover = value; } }
        public int Position { get { return (int)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }
        public double Offset { get { return (double)GetValue(OffsetProperty); } set { SetValue(OffsetProperty, value); } }
        public BookState State { get { return (BookState)GetValue(StateProperty); } set { SetValue(StateProperty, value); } }
        public String CurrentTitle { get { return (String)GetValue(CurrentTitleProperty.DependencyProperty); } }
        public String Author { get; set; }
        public DateTime LastLoadTime { get { return (DateTime)GetValue(LastLoadTimeProperty); } set { SetValue(LastLoadTimeProperty, value); } }
        public static readonly DependencyProperty LastLoadTimeProperty = DependencyProperty.Register("LastLoadTime", typeof(DateTime), typeof(Book));
        public double SortArgument { get { return (double)GetValue(SortArgumentProperty); } set { SetValue(SortArgumentProperty, value); } }
        public static readonly DependencyProperty SortArgumentProperty = DependencyProperty.Register("SortArgument", typeof(double), typeof(Book));
        public String Id { get { return id; } set { id = value; PluginManager.Execute("TRBookcase", "check", this); } }
        public bool IsOpen = false;
        public String Md5 { get; set; }
        public IContentItemAdapter Chapter { get { return null; } }
        public IContentItemAdapter CurrentChapter { get; set; }
        static Book() {
        }

        public Book()
            : base() {
            CurrentChapter = this;
            IsLoaded = false;
            Position = 0; Offset = 0;
            Node = new LinkedListNode<IContentItemAdapter>(this);
        }

        public Book(String src)
            : this() {
            CurrentChapter = this;
            Id = A.MD5F(src);
            SortArgument = 0;
            Source = src;
            LastLoadTime = default(DateTime);
        }

        public Book(IBook b) {
            CurrentChapter = this;
            Source = b.Source;
            Cover = b.Cover;
            LastLoadTime = b.LastLoadTime;
            Position = b.Position;
            Offset = b.Offset;
            Title = b.Title ?? Title;
            Author = b.Author ?? Author;
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

        private Chapter Insert(ZMECapture[] captures, int level, Chapter node) {
            Chapter ret = null;
            if (level > 0) node.Number = captures[level - 1].Numbers.Length > 0 ? (int?)captures[level - 1].Numbers[0] : null;
            if (level < captures.Count()) {
                ret = Insert(captures, level + 1, (Chapter)node[captures[level].Text]);
            } else {
                ret = node;
            }
            return ret;
        }        

        public bool Match(IEnumerable<String> texts) {
            Chapter node = this;
            ZME listZme = null;
            if (Dispatcher.Invoke(() => { return Rule.Instance.IsListEnable; })) 
                listZme = Dispatcher.Invoke(() => { return Rule.Instance.ListTrmex; });
            ZME treeZme = null;
            if (Dispatcher.Invoke(() => Rule.Instance.IsTreeEnable))
                treeZme = Dispatcher.Invoke(() => Rule.Instance.TreeTrmex);
            foreach (var s in texts) {
                if (!IsOpen) return false;
                bool b = true;
                do {
                    //匹配
                    if (s.IsNullOrWhiteSpace()) break;
                    var r = listZme != null ? listZme.Match(s) : null;
                    if (r == null && treeZme != null) r = treeZme.Match(s);
                    if (r == null) break;
                    //插入
                    if (r.Depth <= 1) {
                        //if (node != null) Dispatcher.Invoke(() => { node.Length = 0; var l = node.Length; });
                        //node = Dispatcher.Invoke(() => { return Insert(r.Captures, 0, this); });
                        Stack<Chapter> ns = new Stack<Chapter>();
                        Chapter n = null;
                        Dispatcher.Invoke(() => { for (n = node; n != this; n = n.Parent as Chapter) ns.Push(n); });
                        var e=r.Captures.GetEnumerator();
                        n = null;
                        bool nullcapture = false;
                        while (true) {
                            if (!e.MoveNext()) {
                                nullcapture = true;
                                break;
                            }
                            if (ns.IsEmpty()) break;
                            n = ns.Pop();
                            ZMECapture zc = e.Current as ZMECapture;
                            var ntitle = Dispatcher.Invoke(() => n.Title);
                            if (ntitle.Contains(zc.Text)) 
                                continue;
                            if (zc.Text.Contains(ntitle)) {
                                Dispatcher.Invoke(() => n.Title = zc.Text);
                                continue;
                            }
                            break;
                        }
                        
                        if (!nullcapture) {
                            Chapter cn = n == null ? this : n.Parent as Chapter;    
                            do {
                                ZMECapture zc = e.Current as ZMECapture;
                                cn = Dispatcher.Invoke(() => {
                                    var rn = cn.AppendSub(zc.Text) as Chapter;
                                    rn.Number = !zc.Numbers.IsEmpty() ? (int?)zc.Numbers[0] : null;
                                    return rn;
                                });
                            } while (e.MoveNext());
                            node = cn;
                        } else {
                            node = n;
                        }
                        
                    } else {
                        var n = node;
                        while (n.Children != null) n = n.Children.Last.Value as Chapter;
                        while (n.Level < r.Position - 1) n = n["未命名章节"];
                        while (n.Level > r.Position - 1 && n != null) n = n.Parent as Chapter;
                        if (node != null) Dispatcher.Invoke(() => { node.Length = 0; var l = node.Length; });
                        node = n[r.Text];
                        node.Number = r.Captures[node.Level - 1].Numbers.Length > 0 ? (int?)r.Captures[node.Level - 1].Numbers[0] : null;
                    }
                    b = false;
                } while (false);
                if (b) node.AppendText(s);
            }
            return true;
        }

        public void Load() {
            Load(null);
        }

        public async void Load(String file) {
            if (file == null) file = Source; else Source = file;
            if (State == BookState.Local) {
                Clear();
                var ss = File.ReadAllLines(file, Encoding.Default);
                Title = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file));
                if (Text != null) Text.Clear();
                if (File.Exists(Source)) {
                    TotalText = ss.ToList();
                    //await BookParser.LoadAsync(this);
                    LastLoadTime = DateTime.Now;
                    IsOpen = true;
                    bool matchresult = await TaskEx.Run(() => Match(TotalText));
                    TotalText = null;
                    if (matchresult) Update();
                } else {
                    State = BookState.Missing;
                    Update();
                }
                GenerateIndex();
                TotalText = null;

                PluginManager.Execute("TRContent", "+SelectChange", (ContentSelectedItemChangedEventHandler)Instance_SelectedItemChanged);
                Dispatcher.Invoke(() => {
                    if (Loaded != null) Loaded(this, new PluginEventArgs().Add("state", "all"));
                    //if (Empty.LoadFinished != null) Empty.LoadFinished(this, EventArgs.Empty);
                });
            } else {
                //await Download();
                //if (State == BookState.Local) Load(Source);
            }
            IsLoaded = true;
        }

        void Instance_SelectedItemChanged(object sender, ContentSelectedItemChangedEventArgs e) {
            Position = e.Item.AbsolutePosition;
            Offset = 0;
        }


        public String Source {
            get { return source; }
            set {
                source = value;
                if (source.IsNullOrWhiteSpace()) State = BookState.Missing;
                else if (State == BookState.Local) {
                    if (!(source.Contains('\\') || source.Contains('/'))) State = BookState.Remote;
                    else if (!File.Exists(source)) State = BookState.Missing;
                } else {
                    if (File.Exists(source)) State = BookState.Local;
                }
                if (Title.IsNullOrWhiteSpace()) {
                    Title = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(source));                    
                }
                PluginManager.Execute("TRBookcase", "check", this);
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

       

        //public void MoreInfo() {
        //    if (PluginManager.Instance["TRSpider"] != null) BookInfoManager.MoreInfo(this);
        //    else Douban.MoreInfo(this);
        //}

        private List<IContentItemAdapter> positions;
        public IContentItemAdapter[] Positions;
        private void GenerateIndex() {
            positions = new List<IContentItemAdapter>();
            GenerateIndex(this);
            Positions = positions.ToArray();
        }

        private void GenerateIndex(IContentItemAdapter node) {
            if (node.Children != null && node.Children.Count() > 0) {
                foreach (var e in node.Children) GenerateIndex(e);
            } else {
                positions.Add(node);
            }
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d.IsNull()) return;
            Debug.WriteLine("pos changed: " + e.OldValue + " -> " + e.NewValue);
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
            if (o.PositionChanged.NotNull()) o.PositionChanged(o, new PluginEventArgs());            
        }

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
            if (d.IsNull()) return;
            Debug.WriteLine("off changed: " + e.OldValue + " -> " + e.NewValue);
            var o = d as Book;
            if (o.OffsetChanged.NotNull()) o.OffsetChanged(o, new PluginEventArgs());
        }

        private static object PositionCeorce(DependencyObject d, object baseValue) {
            int x = (int)baseValue;
            Book b = d as Book;
            if (!b.IsLoaded) return baseValue;
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
            Positionable bmk = new Positionable(this);
            while (undoList.Count > undoPos) undoList.RemoveAt(undoList.Count - 1);
            undoList.Add(bmk);
            undoPos = undoList.Count;
            dodoLock = false;
        }

        public override void Close() {
            if (this.IsNull()) return;
            if (Closing != null) Closing(this, new PluginEventArgs());
            PluginManager.Execute("TRContent", "-SelectChange", (ContentSelectedItemChangedEventHandler)Instance_SelectedItemChanged);
            positions = null;
            var p = Preview;
            Loaded = null;
            IsOpen = false;
            base.Close();
            if (Closed != null) Closed(this, new PluginEventArgs());            
        }

        public static void Reopen(){
            if ((G.Book as Book).IsNull()) return;
            String f = (G.Book as Book).source;
            G.Book = null;
            G.Book = new Book(f);
        }

        public static void Open(String filename) {
            if ((G.Book as Book).NotNull() && (G.Book as Book).Source == filename) return;
            G.Book = new Book(filename);
        }

        public static void Open(IBook b) {
            if (b == null) return;
            if ((G.Book as Book).NotNull() && (G.Book as Book).Source == b.Source) return;
            G.Book = new Book(b);
        }

        void IBook.Open(object filename) { Book.Open((String)filename); }

        void IBook.Reopen() { Book.Reopen(); }

        public void Delete() {
            //if (!Source.IsNullOrEmpty())
            //    File.Delete(BookParser.GetBookPath(this));
        }

        public override void Update() {
            base.Update();
            var p = Preview;
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(e.Property.Name));

        }
    }
}