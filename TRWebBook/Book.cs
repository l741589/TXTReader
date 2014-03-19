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
using TRSpider;
using System.Windows.Media.Imaging;
using Zlib.Text.Xml;
using System.ComponentModel;
using System.Web;

namespace TRWebBook {

    internal partial class Book : Chapter, IContentAdapter, IBook, IXmlParsable {
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
        private String id = null;
        private static ZTask uploadTask = new ZTask();
        private static ZTask downloadTask = new ZTask();
        private int undoPos = 0;
        private bool IsLoaded { get; set; }

        public String Cover { get { if (cover.IsNull()) return Entry.NO_COVER; else return cover; } set { cover = value; } }
        public override int Position { get { return (int)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }
        public override double Offset { get { return (double)GetValue(OffsetProperty); } set { SetValue(OffsetProperty, value); } }
        public BookState State { get { return (BookState)GetValue(StateProperty); } set { SetValue(StateProperty, value); } }
        public String CurrentTitle { get { return (String)GetValue(CurrentTitleProperty.DependencyProperty); } }
        public IContentItemAdapter CurrentChapter { get; set; }
        public String Author { get; set; }
        public DateTime LastLoadTime { get { return (DateTime)GetValue(LastLoadTimeProperty); } set { SetValue(LastLoadTimeProperty, value); } }
        public static readonly DependencyProperty LastLoadTimeProperty = DependencyProperty.Register("LastLoadTime", typeof(DateTime), typeof(Book));
        public double SortArgument { get { return (double)GetValue(SortArgumentProperty); } set { SetValue(SortArgumentProperty, value); } }
        public static readonly DependencyProperty SortArgumentProperty = DependencyProperty.Register("SortArgument", typeof(double), typeof(Book));
        public String Id { get { return id; } set { id = value; } }
        public bool IsOpen = false;
        public String Md5 { get; set; }
        private BookDownloader Downloader { get; set; }
        private SpiderClosure Spider { get { return Downloader.StdSpider; } }
        private IEnumerable<SpiderClosure> Spiders { get { return Downloader.Spiders; } }
        protected override Book Root { get { return this; } }
        private bool hasContent = false;
        public String Source { get { return (String)GetValue(SourceProperty); } set { SetValue(SourceProperty, value); } }
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(String), typeof(Book));


        public Book()
            : base() {
            IsLoaded = false;
            Position = 0; Offset = 0;
            Node = new LinkedListNode<IContentItemAdapter>(this);
            CurrentChapter = this;
        }

        public Book(String path) {
            Source = path;
            Downloader = null;
        }

        public Book(IBook b) {
            Source = b.Source;
            Downloader = null;
            Cover = b.Cover;
            LastLoadTime = b.LastLoadTime;
            Position = b.Position;
            Offset = b.Offset;
            Title = b.Title ?? Title;
            Author = b.Author ?? Author;
        }

        public Book(BookDownloader downloader)
            : this() {
                Source = null;
                Downloader = downloader;
        }

        private void Init(BookDownloader downloader){
            Downloader = downloader;
            //Downloader.Concurrence = 5;
            var bd = downloader.StdSpider;
            try {
                Cover = bd.BookDesc.CoverUrl;
            } catch {
                Cover = Entry.NO_COVER;
            }
            Title = bd.BookDesc.Title;
            id = bd.BookDesc.ContentUrl;
            Author = bd.BookDesc.Author;
            LastLoadTime = DateTime.Now;
            Downloader.Preparing += Downloader_Preparing;
            Downloader.DownloadingChapter += d_DownloadingChapter;
            Downloader.ContentFetched += Downloader_ContentFetched;
            CurrentChapter = this;
        }

        void Downloader_ContentFetched(object sender, EventArgs e) {
            CalculateChapter(Position);
        }

        public bool Match(IEnumerable<ChapterDesc> chapters) {
            if (hasContent) return true;
            Chapter node = this;
            ZME listZme = null;
            if (Dispatcher.Invoke(() => Rule.Instance.IsListEnable))
                listZme = Dispatcher.Invoke(() => Rule.Instance.ListTrmex);
            ZME treeZme = null;
            if (Dispatcher.Invoke(() => Rule.Instance.IsTreeEnable))
                treeZme = Dispatcher.Invoke(() => Rule.Instance.TreeTrmex);
            foreach (var chapter in chapters) {
                if (!IsOpen) return false;
                //匹配
                var s = chapter.Title;
                if (s.IsNullOrWhiteSpace()) continue;
                var r = listZme != null ? listZme.Match(s) : null;
                if (r == null && treeZme != null) r = treeZme.Match(s);
                if (r == null) {
                    Chapter cn = Dispatcher.Invoke(() => {
                        var rn = AppendSub(s);
                        rn.Number = -1;
                        return rn;
                    });
                    node = cn;
                } else {
                    //插入
                    if (r.Depth <= 1) {
                        //if (node != null) Dispatcher.Invoke(() => { node.Length = 0; var l = node.Length; });
                        //node = Dispatcher.Invoke(() => { return Insert(r.Captures, 0, this); });
                        Stack<Chapter> ns = new Stack<Chapter>();
                        Chapter n = null;
                        Dispatcher.Invoke(() => { for (n = node; n != this; n = n.Parent as Chapter) ns.Push(n); });
                        var e = r.Captures.GetEnumerator();
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
                                    var rn = cn.AppendSub(zc.Text);
                                    rn.Number = !zc.Numbers.IsEmpty() ? (int?)zc.Numbers[0] : null;
                                    return rn;
                                });
                            } while (e.MoveNext());
                            node = cn;
                        } else {
                            Chapter cn = n.Parent as Chapter;
                            do {                                
                                cn = Dispatcher.Invoke(() => {
                                    var rn = cn.AppendSub(n.Title);
                                    rn.Number = n.Number;
                                    return rn;
                                });
                            } while (e.MoveNext());
                            node = cn;
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
                }
                node.ChapterDesc = chapter;
                node.AppendText("(等待加载中)");
                node.AppendText("");
            }
            hasContent = true;
            return true;
        }

        public async void Load() {
            Clear();
            if (Downloader == null && Source != null) {
                Downloader = new BookDownloader(1);
                XmlParser.Read(Source, this, false);
            }
            if (Downloader == null) return;
            Init(Downloader);
            //var cs = await TaskEx.Run(() => Spider.GetContent(Spider.BookDesc.ContentUrl));
            //var di = new DownloadInfo();
            //di.Start(Downloader);
            var di = PluginManager.Execute("TRWebBook", "+downloadinfo", Downloader);
            Downloader.DownloadAsync(Title);

            var cs = Downloader.StdSpider.Chapters != null && Downloader.StdSpider.Chapters.Count() > 0 ? Downloader.StdSpider.Chapters
                : await ZEventTask.Wait<List<ChapterDescEx>>(Downloader, "Preparing", r => {
                    r.Handler = new PreparingEventHandler((d, e) => {
                        Dispatcher.Invoke(() => {
                            if (e.Spider == Downloader.StdSpider) {
                                r.Continue(e.Spider.Chapters);
                            }
                        });
                    });
                });
            Title = Path.GetFileNameWithoutExtension(Spider.BookDesc.Title);
            if (Text != null) Text.Clear();
            if (cs != null) {
                var ss = from c in cs select c.Title;
                TotalText = ss.ToList();
                LastLoadTime = DateTime.Now;
                IsOpen = true;
                bool matchresult = await TaskEx.Run(() => Match(cs));
                TotalText = null;
                if (matchresult) Update();
            } else {
                State = BookState.Missing;
                Update();
            }
            GenerateIndex();
            TotalText = null;
            ContentTreePanel.Instance.SelectedItemChanged += Instance_SelectedItemChanged;
            Dispatcher.Invoke(() => {
                if (Loaded != null) Loaded(this, new PluginEventArgs().Add("state", "init"));
            });
            await ZEventTask.Wait(Downloader, "Done", r => { if (!Downloader.IsDone) r.Handler = new EventHandler((d, e) => r.Continue()); });
            IsLoaded = true;
            PluginManager.Execute("TRWebBook", "-downloadinfo", di);
            Length = 0;
            var l = Length;
        }


        void Downloader_Preparing(object sender, PreparingEventArgs e) {
            
        }

        void d_DownloadingChapter(object sender, DownloadingChapterEventArgs e) {
            if (e.AlreadyDone) return;
            switch (e.State) {
                case DownloadingChapterEventArgs.States.StandardSuccess:
                case DownloadingChapterEventArgs.States.NonstandardSuccess:
                    var ss = e.RealChapter.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    Dispatcher.Invoke(() => {
                        var c = FindChapter(e.StandardChapter);
                        if (c == null) return;
                        dodoLock = true;
                        var p = new Positionable(CurrentChapter as IPositionable);
                        c.Text = ss.ToList();
                        c.ChapterDesc = e.RealChapter;
                        c.RefreshTotalText();
                        AbsolutePosition = -1;
                        p.AssignTo(this);
                        ChapterLoadFinished(new PluginEventArgs().Add("state", "chapter").Add("chapter", c));
                        dodoLock = false;
                        c.Length = 0;
                        var l = c.Length;
                    });
                    break;
                case DownloadingChapterEventArgs.States.AllFail:
                    Dispatcher.Invoke(() => {
                        var c = FindChapter(e.StandardChapter);
                        if (c == null) return;
                        c.Text = new String[] { "(加载失败）" }.ToList();
                        c.RefreshTotalText();
                        if (Loaded != null) Loaded(this, new PluginEventArgs().Add("state", "chapter").Add("chapter", c));
                        c.Length = 0;
                        var l = c.Length;
                    });
                    break;
            }
        }

        private bool chapterloadlock = false;
        private bool chapterloadredo = false;
        private async void ChapterLoadFinished(PluginEventArgs args) {
            do{
                chapterloadredo = false;
                if (Loaded != null) {
                    if (chapterloadlock) {
                        chapterloadredo = true;
                        return;
                    }
                    chapterloadlock = true;
                    Loaded(this, args);
                    await 1000.Wait();                    
                    chapterloadlock = false;
                }
            }while(chapterloadredo);
            chapterloadlock = false;
        }

        void Instance_SelectedItemChanged(object sender, ContentSelectedItemChangedEventArgs e) {
            Position = e.Item.AbsolutePosition;
            Offset = 0;
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
                if (Id != null && Id != "") ret += Id + "\n";
                //if (Preview != null) ret += "内容：\n" + Preview + "\n";
                return ret.Trim();
            }
        }

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

        private void CalculateChapter(int pos){
            if (Positions != null) {
                int i = pos;
                int l = 0, r = Positions.Length;
                int m = (l + r) >> 1;
                while (true) {
                    var p = Positions[m];
                    if (i < p.AbsolutePosition) {
                        r = m;
                    } else if (i >= p.AbsolutePosition) {
                        if (m == r - 1) break;
                        var q = Positions[m + 1];
                        if (i < q.AbsolutePosition) {
                            break;
                        } else {
                            l = m;
                        }
                    }
                    m = (l + r) >> 1;
                    if (m == 0) break;
                }
                if (positions != null) {
                    if (CurrentChapter != positions[m]) {
                        CurrentChapter = positions[m] as Chapter;
                        if ((CurrentChapter as Chapter).ChapterDesc != null) {
                            Downloader.CircuteToTop((CurrentChapter as Chapter).ChapterDesc);
                        }
                    }
                }
                SetValue(Book.CurrentTitleProperty, CurrentChapter.TotalTitle);
            } else {
                CurrentChapter = this;
                SetValue(Book.CurrentTitleProperty, TotalTitle);
            }
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d.IsNull()) return;
            Debug.WriteLine("pos changed: " + e.OldValue + " -> " + e.NewValue);
            var o = d as Book;
            o.CalculateChapter((int)e.NewValue);
            if (Math.Abs((int)e.OldValue - (int)e.NewValue) > DO_GATE) {
                o.DoDo();
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

        public override void Close() {
            if (this.IsNull()) return;
            if (Source.IsNullOrWhiteSpace()) Source = TXTReader.G.PATH_SOURCE + Title + "." + DateTime.Now.ToString("yyyyMMddhhmssfff") + ".trbx";
            XmlParser.Write(Source, this);
            if (Closing != null) Closing(this, new PluginEventArgs());            
            ContentTreePanel.Instance.SelectedItemChanged -= Instance_SelectedItemChanged;
            positions = null;
            var p = Preview;
            Loaded = null;
            hasContent = false;
            IsOpen = false;
            Downloader.ShutdownAsync();
            base.Close();
            if (Closed != null) Closed(this, new PluginEventArgs());            
        }

        public static void Open(BookDownloader downloader) {
            downloader.Reset();
            TXTReader.G.Book = new Book(downloader);
        }

        public static void Open(String path) {
            TXTReader.G.Book = new Book(path);
        }

        public static void Open(IBook book) {
            TXTReader.G.Book = new Book(book);
        }

        void IBook.Open(object bd) {
            if (bd is BookDownloader) Book.Open((BookDownloader)bd);
            else if (bd is IBook) Book.Open((IBook)bd);
            else Book.Open(bd.ToString());
        }

        public void Reopen() {
            Close();
            TXTReader.G.Book = new Book(Downloader);
        }

        public void Delete() {
            
        }

        public override void Update() {
            base.Update();
            var p = Preview;
        }


        public bool CanUndo { get { return undoPos > 1 && !dodoLock; } }
        private List<Positionable> undoList = new List<Positionable>();
        private bool dodoLock = false;
        public bool CanRedo { get { return undoPos < undoList.Count && !dodoLock; } }

        public void Undo() {
            Debug.WriteLine("UNDO:" + CanUndo);
            if (!CanUndo) return;
            dodoLock = true;
            --undoPos;
            undoList[undoPos - 1].AssignTo(this);
            //G.Displayer.Update();
            dodoLock = false;
        }       

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
            Positionable bmk = new Positionable(CurrentChapter as IPositionable);
            while (undoList.Count > undoPos) undoList.RemoveAt(undoList.Count - 1);
            undoList.Add(bmk);
            undoPos = undoList.Count;
            dodoLock = false;
        }


        public XmlParserReaderCallback Read {
            get {
                return r => {
                    r = r.Child("editable_book")
                    .Do(Downloader)
                    .Do(new ContentParser(this));
                    return r.Parent;
                };
            }
        }

        public XmlParserWriterCallback Write {
            get {
                return w => w.Start("editable_book")
                    .Do(Downloader)
                    .Do(new ContentParser(this))
                    .End;
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(e.Property.Name));
        }
    }
}