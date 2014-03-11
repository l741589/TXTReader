using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TXTReader;
using Zlib.Utility;
using Zlib.UI.Utility;
using Zlib.UI;
using System.IO;
using Zlib.Algorithm;
using System.Web;
using System.Windows.Controls.Primitives;
using Zlib.Win32;

namespace TRSpider {

    static class Dicts {
        public static Dictionary<DownloadingChapterEventArgs.States, String> StateDict = new Dictionary<DownloadingChapterEventArgs.States, String>();
        public static ObservableCollection<String> Sites = new ObservableCollection<String>();
        public static ObservableCollection<int> Threads = new ObservableCollection<int>();
        static Dicts() {
            StateDict[DownloadingChapterEventArgs.States.AllFail] = "失败";
            StateDict[DownloadingChapterEventArgs.States.MatchFail] = "匹配失败";
            StateDict[DownloadingChapterEventArgs.States.NonstandardFail] = "尝试失败";
            StateDict[DownloadingChapterEventArgs.States.StandardFail] = "尝试失败*";
            StateDict[DownloadingChapterEventArgs.States.MatchSuccess] = "匹配成功";
            StateDict[DownloadingChapterEventArgs.States.StandardSuccess] = "成功";
            StateDict[DownloadingChapterEventArgs.States.NonstandardSuccess] = "成功*";
            StateDict[DownloadingChapterEventArgs.States.Exception] = "异常";
            StateDict[DownloadingChapterEventArgs.States.ValidateFail] = "验证失败";
        }

        
    }
    /// <summary>
    /// DownloadWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadWindow : Window {

        public enum States { Prepare, Downloading, Success, Fail, Pause };
        private ObservableCollection<SpiderClosure> Spiders = new ObservableCollection<SpiderClosure>();
        private SpiderClosure Standard;
        private BookDownloader Downloader;
        private int idcnt = 0;
        private ListCollectionView Logs = new ListCollectionView(new ObservableCollection<DGItem>());      

        public DownloadWindow(IEnumerable spiders, SpiderClosure std) {
            InitializeComponent();
            //Owner = G.MainWindow;
            foreach (var e in spiders) {
                var o = (e as SpiderClosure).Clone() as SpiderClosure;
                Spiders.Add(o);
                if (e == std) {
                    Standard = o;
                    o.IsStandard = true;
                }
                if (std.BookDesc != null) {
                    if (o.BookDesc.Title == std.BookDesc.Title && (o.BookDesc.Author == std.BookDesc.Author || std.BookDesc.Author.IsNullOrWhiteSpace())) {
                        o.IsInUse = true;
                    } else {
                        o.IsInUse = false;
                    }
                } else o.IsInUse = false;
            }
            if (Standard == null) {
                if (Standard == null) throw new ApplicationException("标准爬虫必须在位于爬虫列表内");
            }
            Downloader = new BookDownloader(Spiders, Standard);
            pn_spiders.ItemsSource = Spiders;
            Downloader.ContentFetched += Downloader_ContentFetched;
            Downloader.DownloadingChapter += Downloader_DownloadingChapter;
            Loaded += DownloadWindow_Loaded;
            Closing += DownloadWindow_Closing;
            State = States.Prepare;
            pp_fstate.Target = bn_fstate;
            pp_ffrom.Target = bn_ffrom;
            pp_fthread.Target = bn_fthread;
            dg.ItemsSource = Logs;
            Logs.Filter = filter;
        }

        private States state;
        private States State {
            get { return state; }
            set {
                state = value;
                switch (state) {
                    case States.Success: tb_status.Text = "完成"; break;
                    case States.Fail: tb_status.Text = "失败"; MessageBox.Show("下载失败"); break;
                    case States.Downloading: tb_status.Text = "下载中"; break;
                    case States.Prepare: tb_status.Text = "准备中"; break;
                    case States.Pause: tb_status.Text = "暂停中"; break;
                }
            }
        }        

        private bool filter(object item) {
            var i=item as DGItem;
            if (stateFilter != null && !stateFilter.Contains(i.RealState)) return false;
            if (i.From!=null&&siteFilter != null && !siteFilter.Contains(i.From)) return false;
            if (threadFilter != null && !threadFilter.Contains(i.Thread)) return false;
            return true;
        }

        public HashSet<DownloadingChapterEventArgs.States> stateFilter = null;
        public HashSet<String> siteFilter = null;
        public HashSet<int> threadFilter = null;

        void Downloader_DownloadingChapter(object sender, DownloadingChapterEventArgs e) {
            Dispatcher.Invoke(() => {
                var i = new DGItem(++idcnt, e);
                var b = dg.IsScrollAtBottom(true);
                if (Logs.CanAddNew) Logs.AddNew();
                Logs.AddNewItem(i);
                if (i.From!=null&&!Dicts.Sites.Contains(i.From)) Dicts.Sites.Add(i.From);
                if (!Dicts.Threads.Contains(i.Thread)) Dicts.Threads.Add(i.Thread);
                Logs.CommitNew();
                if (b) dg.ScrollToBottom();
                if (e.StandardChapter != null && e.RealChapter != null && e.StandardChapter != e.RealChapter) e.StandardChapter.Text = e.RealChapter.Text;
            });
        }

        void DownloadWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (State == States.Fail || State == States.Success) return;
            if (MessageBox.Show(this, "退出此窗口将结束下载，确定退出？", "确认退出", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                Downloader.ShutdownAsync();
            } else {
                e.Cancel = true;
            }
        }

        async void DownloadWindow_Loaded(object sender, RoutedEventArgs e) {
            this.SetForegroundWindow();
            await Downloader.Download(Standard.BookDesc.Title);
            CheckResult();
        }

        private void CheckResult() {
            if (State != States.Pause) {
                if (Standard == null || Standard.Chapters == null) {
                    MessageBox.Show(this, "下载失败");
                    return;
                }
                State = States.Success;
                int success = 0;
                int count = 0;
                int done = 0;
                foreach (var c in Standard.Chapters) {
                    ++count;
                    if (c.StandardState == ChapterDescEx.StandardStates.NonStandardSuccess || c.StandardState == ChapterDescEx.StandardStates.Success) {
                        ++success;
                    } else {
                        if (c.IsManual) ++done;
                    }
                }
                if (!IsVisible) return;
                Save();
                MessageBox.Show(this,
                    String.Format("下载完成！\r\n共下载{0}个章节\r\n成功{1}个章节\r\n失败{2}个章节\r\n可双击目录中的章节进行修正。"
                    , count, success, count - success));
            } else {
                int success = 0;
                int count = 0;
                int done = 0;
                foreach (var c in Standard.Chapters) {
                    ++count;
                    if (c.StandardState == ChapterDescEx.StandardStates.NonStandardSuccess || c.StandardState == ChapterDescEx.StandardStates.Success) {
                        ++success;
                    } else {
                        if (c.IsManual) ++done;
                    }
                }
                if (!IsVisible) return;
                Save();
                MessageBox.Show(this,
                    String.Format("下载暂停！\r\n共下载{0}个章节\r\n成功{1}个章节\r\n失败或未开始{2}个章节\r\n可双击目录中的章节进行修正。"
                    , count, success, count - success));
            }
        }

        void Downloader_ContentFetched(object sender, EventArgs e) {
            if (Standard.IsFailed) {
                State = States.Fail;
            } else {
                lv_chapter.ItemsSource = Standard.Chapters;
                State = States.Downloading;
            }
        }

        private void sp_MouseMove(object sender, MouseEventArgs e) {
            if (sp.IsMouseCaptured) {
                var p = e.GetPosition(lv_chapter);
                pn_left.Width = Math.Max(8, p.X - 2);
            }
        }

        private void sp_MouseDown(object sender, MouseButtonEventArgs e) {
            sp.CaptureMouse();
        }

        private void sp_MouseUp(object sender, MouseButtonEventArgs e) {
            sp.ReleaseMouseCapture();
        }

        class DGItem {
            public String State { get; set; }
            public DownloadingChapterEventArgs.States RealState { get; set; }
            public String From { get; set; }
            public String TitleStd { get; set; }
            public String TitleReal { get; set; }
            public String TextStd { get; set; }
            public String TextReal { get; set; }
            public String Exception { get; set; }
            public int Id { get; set; }
            public int Thread { get; set; }

            public DGItem(int id, DownloadingChapterEventArgs e) {
                RealState = e.State;
                State = Dicts.StateDict[e.State];
                Id = id;
                From = e.SpiderClosure == null ? null : e.SpiderClosure.Name;
                TitleStd = e.StandardChapter == null ? null : e.StandardChapter.Title;
                TitleReal = e.RealChapter == null ? null : e.RealChapter.Title;
                Exception = e.Exception == null ? null : e.Exception.Message;
                Thread = e.Thread;
                var textStd=e.StandardChapter == null || e.StandardChapter.Text.IsNullOrWhiteSpace() ? null : e.StandardChapter.Text;
                if (textStd.IsNullOrWhiteSpace() || textStd.StartsWith(TRZSS.ERROR_HEADER)) textStd = "##预览##" + e.StandardChapter.Preview;
                TextStd = textStd;
                TextReal = e.RealChapter == null || e.RealChapter.Text.IsNullOrWhiteSpace() ? null : e.RealChapter.Text;
            }
        }

        private void bn_fstate_Click(object sender, RoutedEventArgs e) {
            pp_fstate.IsOpen = true;
        }

        public void MultiSelectPanel_MultiSelectionChanged(object sender, MultiSelectionChangedEventArgs e) {
            if (stateFilter == null) stateFilter = new HashSet<DownloadingChapterEventArgs.States>();
            stateFilter.Clear();
            foreach (KeyValuePair<DownloadingChapterEventArgs.States, String> x in e.SelectedItems) {
                stateFilter.Add(x.Key);
            }
            Logs.Refresh();
        }

        private void MultiSelectPanel_MultiSelectionChanged_1(object sender, MultiSelectionChangedEventArgs e) {
            if (siteFilter == null) siteFilter = new HashSet<String>();
            siteFilter.Clear();
            foreach (String x in e.SelectedItems) {
                siteFilter.Add(x);
            }
            Logs.Refresh();
        }

        private void MultiSelectPanel_MultiSelectionChanged_2(object sender, MultiSelectionChangedEventArgs e) {
            if (threadFilter == null) threadFilter = new HashSet<int>();
            threadFilter.Clear();
            foreach (int x in e.SelectedItems) {
                threadFilter.Add(x);
            }
            Logs.Refresh();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                var cd = (sender as FrameworkElement).DataContext as ChapterDescEx;
                var w = new ChapterDisplayWindow(cd.IsManual ? cd.ManualText : cd.Text);
                if (w.ShowDialog() == true) {
                    cd.ManualText = w.tb.Text;
                    Save();
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            var cd = (sender as FrameworkElement).DataContext as ChapterDescEx;
            cd.ManualText = null;
        }

        String LastFile = null;
        public void Save() {
            try {
                if (State != States.Success) return;
                if (Standard == null || Standard.Chapters == null || Standard.BookDesc == null) return;
                StringBuilder sb = new StringBuilder();
                sb.Append(Standard.BookDesc.Title);
                if (!Standard.BookDesc.Author.IsNullOrWhiteSpace()) sb.Append(" - ").Append(Standard.BookDesc.Author).Append("\r\n");
                if (!Standard.BookDesc.Description.IsNullOrEmpty()) sb.Append(Standard.BookDesc.Description).Append("\r\n");
                sb.Append("\r\n");
                foreach (var e in Standard.Chapters)
                {
                    sb.Append(e.Title).Append("\r\n").Append(e.IsManual ? e.ManualText : e.Text).Append("\r\n");
                }
                var s=sb.ToString();
                String path = G.PATH_SOURCE + Standard.BookDesc.Title + "."+ DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".txt";
                File.WriteAllText(path, s, Encoding.UTF8);
                if (LastFile != null) File.Delete(LastFile);
                LastFile = path;
            } catch { }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) {
            var cd = (sender as FrameworkElement).DataContext as ChapterDescEx;
            try {
                if (cd != null && !cd.TextUrl.IsNullOrWhiteSpace()) System.Diagnostics.Process.Start(Standard.BookDesc.ContentUrl);
                else MessageBox.Show("无效的链接");
            } catch { }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e) {
             var cd = (sender as FrameworkElement).DataContext as ChapterDescEx;
            try {
                if (cd != null && !cd.TextUrl.IsNullOrWhiteSpace()) System.Diagnostics.Process.Start(cd.TextUrl);
                else MessageBox.Show("无效的链接");
            } catch { }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e) {
            var cd = (sender as FrameworkElement).DataContext as ChapterDescEx;
            try {
                if (cd != null && !cd.TextUrl.IsNullOrWhiteSpace()) {
                    var fmt = "http://www.baidu.com/s?wd={0}+{1}";
                    System.Diagnostics.Process.Start(String.Format(fmt, HttpUtility.UrlDecode(Standard.BookDesc.Title), HttpUtility.UrlEncode(cd.Title)));
                } else MessageBox.Show("无效的链接");
            } catch { }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e) {
            var cd = (sender as FrameworkElement).DataContext as ChapterDescEx;
            try {
                if (cd != null && !cd.TextUrl.IsNullOrWhiteSpace()) {
                    var fmt = "https://www.google.com/search?q={0}+{1}";
                    System.Diagnostics.Process.Start(String.Format(fmt, HttpUtility.UrlDecode(Standard.BookDesc.Title), HttpUtility.UrlEncode(cd.Title)));
                } else MessageBox.Show("无效的链接");
            } catch { }
        }

        private async void ToggleButton_Click(object sender, RoutedEventArgs e) {
            var b = sender as ToggleButton;
            b.IsEnabled = false;
            if (Downloader.IsDownloading) {
                await Downloader.Shutdown();
                State = States.Pause;
            } else {
                State = States.Prepare;
                await Downloader.Download(null);                
            }            
            CheckResult();
            b.IsEnabled = true;
        }
    }
}
