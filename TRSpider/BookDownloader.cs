using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Utility;
using Zlib.Async;
using System.Collections;
using System.Diagnostics;
using Zlib.Algorithm;
using Zlib.Text;
using Zlib.Text.Xml;
using System.Xml;

namespace TRSpider {

    public delegate void DownloadingChapterEventHandler(object sender, DownloadingChapterEventArgs e);
    public class DownloadingChapterEventArgs : EventArgs {
        public enum States { StandardSuccess, NonstandardSuccess, MatchFail, MatchSuccess, ValidateFail, StandardFail, NonstandardFail, AllFail, Exception };
        public States State { get; set; }
        public ChapterDescEx StandardChapter { get; set; }
        public ChapterDescEx RealChapter { get; set; }
        public BookDesc BookDesc { get; set; }
        public SpiderClosure SpiderClosure { get; set; }
        public Exception Exception { get; set; }
        public int Thread { get; set; }
        public bool AlreadyDone { get; set; }
    }

    public delegate void PreparingEventHandler(object sender, PreparingEventArgs e);
    public class PreparingEventArgs : EventArgs {
        public SpiderClosure Spider { get; set; }
        public bool AlreadyDone { get; set; }
        public Exception Exception { get; set; }
    }

    public class BookDownloader : IXmlParsable {

        public List<SpiderClosure> Spiders { get; private set; }
        public SpiderClosure StdSpider { get; private set; }
        public event EventHandler ContentFetched;
        public event DownloadingChapterEventHandler DownloadingChapter;
        public event PreparingEventHandler Preparing;
        public event EventHandler Done;
        public bool IsDone { get; set; }
        public bool IsDownloading { get; set; }
        private ZMultiTask task = null;
        private bool shutedDown = false;
        public int Concurrence { get; set; }
        private ZMultiTask.Task zmtask = null;

        public BookDownloader(int concurrence = 5) {
            Concurrence = concurrence;
            IsDone = false;
            shutedDown = false;
            Spiders = new List<SpiderClosure>();
            StdSpider = null;
        }

        public BookDownloader(IEnumerable Spiders, SpiderClosure std) {
            Concurrence = 5;
            IsDone = false;
            IsDownloading = false;
            this.Spiders = new List<SpiderClosure>();
            this.StdSpider = null;
            foreach (var item in Spiders) {
                this.Spiders.Add((SpiderClosure)item);
                if (item == std) this.StdSpider = std;
            }
            if (this.StdSpider == null) throw new ApplicationException("标准爬虫必须在位于爬虫列表内");
        }

        public BookDownloader(IEnumerable<SpiderClosure> Spiders, SpiderClosure std, int concurrence = 5) {
            if (!Spiders.Contains(std)) throw new ApplicationException("标准爬虫必须在位于爬虫列表内");
            this.StdSpider = std;
            this.Spiders = Spiders.ToList();
            this.Concurrence = concurrence;
            IsDone = false;
            IsDownloading = false;
        }

        public async Task FetchAllContent(String keyWord) {
            List<Task> tasks = new List<Task>();
            foreach (var e in Spiders) {
                tasks.Add(TaskEx.Run(() => {
                    try {
                        e.Search(keyWord);
                        if (e.BookDesc != null) {
                            if (e.Chapters != null) {
                                OnPreparing(e, null, true);
                            } else {
                                e.GetContent(e.BookDesc.ContentUrl);
                                OnPreparing(e);
                            }
                        } else {
                            OnPreparing(e, new Exception("抓取书籍信息失败"));
                        }
                    } catch (Exception ex) {
                        OnPreparing(e, ex);
                    }
                }));
            }
            if (tasks.Count == 0) return;
            await TaskEx.WhenAll(tasks);
            if (ContentFetched != null) ContentFetched(this, EventArgs.Empty);
        }

        public async Task DownloadBook(SpiderClosure std) {
            if (std == null) return;
            if (std.Chapters == null) return;
            task = new ZMultiTask();
            foreach (var e in std.Chapters) {
                task.Add(() => {
                    var cd = DownloadChapter(std, e);
                }, e);
            }

            if (shutedDown) return;
            zmtask = task.Execute(Concurrence,false);
            zmtask.WorkDone += (d, e) => {
                if (e.Success) {
                } else {
                    OnDownloadingChapter(DownloadingChapterEventArgs.States.Exception, std, e.Tag as ChapterDescEx, null, e.Exception);
                }
            };
            await zmtask;
            task = null;
        }

        public Dictionary<int, ChapterDescEx> GenerateChapterHolder() {            
            if (Spiders == null) return null;
            Dictionary<int, ChapterDescEx> ret = new Dictionary<int, ChapterDescEx>();
            foreach (var s in Spiders) {
                if (s.Chapters == null) continue;
                foreach (var c in s.Chapters) {
                    ret[c.SerializeId] = c;
                }
            }
            return ret;
        }



        //输入要保证cd是std的章节
        public ChapterDescEx DownloadChapter(SpiderClosure std, ChapterDescEx cd, bool retryfail = false) {
            if (cd.State == ChapterDescEx.States.Success) {
                OnDownloadingChapter(DownloadingChapterEventArgs.States.StandardSuccess, std, cd, cd, null, true);
                return cd;
            } else {
                if (cd.State == ChapterDescEx.States.Ready || cd.State == ChapterDescEx.States.Pending || 
                    (retryfail && cd.State == ChapterDescEx.States.Fail)) {
                    cd.State = ChapterDescEx.States.Pending;
                    cd.Standard = cd;
                    std.GetText(cd);
                }else if (!retryfail && cd.State == ChapterDescEx.States.Fail){
                    OnDownloadingChapter(DownloadingChapterEventArgs.States.StandardFail, std, cd, cd);
                }
                if (cd.State == ChapterDescEx.States.Success) {
                    OnDownloadingChapter(DownloadingChapterEventArgs.States.StandardSuccess, std, cd, cd);
                    return cd;
                } else {
                    OnDownloadingChapter(DownloadingChapterEventArgs.States.StandardFail, std, cd, cd);
                }
            }
            ChapterDescEx r = cd;
            foreach (var e in Spiders) {
                if (e.IsFailed) continue;
                if (e == std) continue;
                if (!e.IsInUse) continue;
                r = MatchChapter(e, cd);
                if (r == null) continue;
                r.Standard = cd;
                if (r.State == ChapterDescEx.States.Success) {
                    OnDownloadingChapter(DownloadingChapterEventArgs.States.NonstandardSuccess, e, cd, r, null, true);
                    return r;
                } else if (r.State == ChapterDescEx.States.Ready || r.State == ChapterDescEx.States.Pending ||
                    (retryfail && r.State == ChapterDescEx.States.Fail)) {
                        r.State = ChapterDescEx.States.Ready;
                    e.GetText(r);
                }else if (!retryfail && r.State == ChapterDescEx.States.Fail){
                    OnDownloadingChapter(DownloadingChapterEventArgs.States.NonstandardFail, e, cd, r);
                    continue;
                }
                if (r.State == ChapterDescEx.States.Success && !r.Text.IsNullOrWhiteSpace()) {
                    //cd.State = ChapterDescEx.States.NonstandardSuccess;
                    if (!cd.Preview.IsNullOrWhiteSpace()) {
                        var s1 = cd.Preview;
                        if (s1.Length > r.Text.Length) goto ValifateFail;
                        var s2 = r.Text.Substring(0, Math.Min(r.Text.Length, s1.Length * 3));
                        if (StringCompare.LongestCommonSubsequenceLength(s1.ToArray(), s2.ToArray()) < s1.Length / 2) goto ValifateFail;
                    } else {
                        if (r.Text.Length < 500) goto ValifateFail;
                    }
                    OnDownloadingChapter(DownloadingChapterEventArgs.States.NonstandardSuccess, e, cd, r);
                    return r;

                ValifateFail:
                    OnDownloadingChapter(DownloadingChapterEventArgs.States.ValidateFail, e, cd, r);
                }
                OnDownloadingChapter(DownloadingChapterEventArgs.States.NonstandardFail, e, cd, r);
            }
            OnDownloadingChapter(DownloadingChapterEventArgs.States.AllFail, null, cd);
            cd.StandardState = ChapterDescEx.StandardStates.FailAll;
            return r;
        }

        public void CheckInUse() {
            if (StdSpider == null) return;
            if (StdSpider.BookDesc == null) return;
            String t = StdSpider.BookDesc.Title.Trim();
            String a = StdSpider.BookDesc.Author.Trim();
            foreach (var e in Spiders) {
                if (e.BookDesc == null) continue;
                e.IsInUse = (a == null || a == e.BookDesc.Author.Trim()) && t == e.BookDesc.Title.Trim();
            }
        }

        public async Task Download(String keyWord) {
            if (Spiders == null || Spiders.Count == 0) return;
            IsDownloading = true;
            IsDone = false;
            shutedDown = false;
            await FetchAllContent(keyWord);
            if (shutedDown) return;
            CheckInUse();
            if (shutedDown) return;
            await DownloadBook(StdSpider);
            if (Done != null) Done(this, EventArgs.Empty);
            IsDone = true;
            IsDownloading = false;
        }

        public async void DownloadAsync(String keyWord) {
            if (Spiders == null || Spiders.Count == 0) return;
            IsDownloading = true;
            IsDone = false;
            shutedDown = false;
            await FetchAllContent(keyWord);
            if (shutedDown) return;
            CheckInUse();
            if (shutedDown) return;
            await DownloadBook(StdSpider);
            if (Done != null) Done(this, EventArgs.Empty);
            IsDone = true;
            IsDownloading = false;
        }

        public async Task Shutdown() {
            if (task != null)
                await task.Shutdown();
            shutedDown = true;
            IsDownloading = false;
        }

        public async void ShutdownAsync() {
            if (task != null)
                await task.Shutdown();
            shutedDown = true;
            IsDownloading = false;
        }

        public ChapterDescEx MatchChapter(SpiderClosure spider, ChapterDescEx std) {
            var keyWord = std.Title;
            var Chapters = spider.Chapters;
            if (Chapters == null) return null;
            ChapterDescEx cd = null;
            int min = 0x7fffffff;
            foreach (var e in Chapters) {
                var lv = Zlib.Algorithm.StringCompare.LevenshteinDistance(e.Title, keyWord);
                if (lv < min) {
                    min = lv;
                    cd = e;
                }
            }
            if (min > cd.Title.Length / 2) {
                var len = Zlib.Algorithm.StringCompare.LongestCommonSubsequenceLength(cd.Title.ToArray(), keyWord.ToArray());
                if (len < Math.Min(cd.Title.Length, keyWord.Length) * 0.8) {
                    OnDownloadingChapter(DownloadingChapterEventArgs.States.MatchFail, spider, std, cd);
                    return null;
                }
            }
            OnDownloadingChapter(DownloadingChapterEventArgs.States.MatchSuccess, spider, std, cd);
            return cd;
            //DebugWindow.Log(String.Format("匹配失败[{0}]\t[{1}]", keyWord, cd.Title));
            //return null;
        }

        private void OnDownloadingChapter(DownloadingChapterEventArgs.States state, SpiderClosure spider, ChapterDescEx std = null, ChapterDescEx real = null, Exception exception = null, bool alreadyDone = false) {
            if (DownloadingChapter != null) {
                DownloadingChapterEventArgs e = new DownloadingChapterEventArgs {
                    State = state,
                    RealChapter = real,
                    StandardChapter = std,
                    SpiderClosure = spider,
                    Exception = exception,
                    Thread = ZMultiTask.CurrentId,
                    AlreadyDone = alreadyDone
                };
                DownloadingChapter(this, e);
            }
        }

        private void OnPreparing(SpiderClosure spider, Exception e = null, bool alreadyDone = false) {
            if (Preparing != null) {
                var args = new PreparingEventArgs {
                    Spider = spider,
                    Exception = e,
                    AlreadyDone = alreadyDone
                };
                Preparing(this, args);
            }
        }

        public void CircuteToTop(ChapterDesc cd) {
            if (zmtask == null) return;
            zmtask.CircuteToTop(cd);
        }

        public Task RunInNewThread(ChapterDesc cd) {
            if (zmtask == null) return null;
            var w = zmtask.Fetch(cd);
            if (w == null) return null;
            return TaskEx.Run(w);
        }

        public void Reset() {
            foreach (var e in Spiders) {
                e.Reset();
            }
        }



        public XmlParserReaderCallback Read {
            get {
                return r => {
                    r = r.Child("spiders");
                    r.ForChildren("spider", n => {
                        var attr=n.Attributes["name"];
                        if (attr == null) return;
                        String name = attr.Value;
                        ISpider sp = SpiderCollection.Instance[name];
                        if (sp == null) return;
                        SpiderClosure sc = new SpiderClosure(sp);
                        if (Spiders == null) Spiders = new List<SpiderClosure>();
                        Spiders.Add(sc);
                        attr = n.Attributes["std"];
                        if (attr != null && attr.Value != null) {
                            bool b = false;
                            bool.TryParse(attr.Value, out b);
                            if (b) {
                                sc.IsStandard = true;
                                StdSpider = sc;
                            }
                        }
                        r = r.ReadEntity<BookDesc>("bookdesc", o => sc.BookDesc = o);
                        r = r.Child("chapters");
                        r.ForChildren("chapter", nn => r.ReadEntity<ChapterDescEx>(null, o => sc.AddChapter(o)));
                        r = r.Parent;
                    });
                    return r.Parent;
                };
            }
        }

        public XmlParserWriterCallback Write {
            get {
                return w => {
                    w = w.Start("spiders");

                    foreach (var e in Spiders) {
                        w = w.Start("spider").Attr("name", e.Name).Attr("std", e == StdSpider);
                        if (e.BookDesc != null) {
                            w = w.WriteEntity("bookdesc", e.BookDesc);
                            w = w.Start("chapters");
                            if (e.Chapters != null) {
                                foreach (var c in e.Chapters) {
                                    w = w.WriteEntity("chapter", c);
                                }
                            }
                            w = w.End;
                        }
                        w = w.End;
                    }
                    w = w.End;
                    return w;
                };
            }
        }
    }
}
