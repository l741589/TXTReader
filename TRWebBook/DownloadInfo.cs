using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using FloatControls;
using TRSpider;
using Zlib.Utility;

namespace TRWebBook {
    class DownloadInfo : FloatMessage {
        private BookDownloader d;
        public int Total { get { return (int)GetValue(TotalProperty); } set { SetValue(TotalProperty, value); } }
        public static readonly DependencyProperty TotalProperty = DependencyProperty.Register("Total", typeof(int), typeof(DownloadInfo));
        public int Success { get { return (int)GetValue(SuccessProperty); } set { SetValue(SuccessProperty, value); } }
        public static readonly DependencyProperty SuccessProperty = DependencyProperty.Register("Success", typeof(int), typeof(DownloadInfo));
        public int Fail { get { return (int)GetValue(FailProperty); } set { SetValue(FailProperty, value); } }
        public static readonly DependencyProperty FailProperty = DependencyProperty.Register("Fail", typeof(int), typeof(DownloadInfo));
        public String Text { get { return (String)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(DownloadInfo));


        public DownloadInfo() {
            
        }

        public DownloadInfo(BookDownloader d) {
            Start(d);
        }

        public void Start(BookDownloader d) {
            this.d = d;
            IsUseFormat = false;
            d.Preparing += d_Preparing;
            d.ContentFetched += d_ContentFetched;
            d.DownloadingChapter += d_DownloadingChapter;
            Format = "{0}/{1}(失败{2}){3}";
            Name = "下载信息";
            Tag = "DownloadInfo";
            Text = "抓取目录中";
            try {
                Total = d.Spiders.Count;
                Success = Fail = 0;
            } catch (Exception e) {
                Format = null;
                Text = e.Message;
            }
            this.Register();
        }

        public void Stop() {
            d = null;
            this.UnRegister();
        }

        private static DependencyProperty[] Caps = { TotalProperty, SuccessProperty, FailProperty, TextProperty, FormatProperty };

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            if (Caps.Contains(e.Property)) {
                if (Format == null) Value = Text;
                else Value = String.Format(Format, Success, Total, Fail, Text);
            }
            base.OnPropertyChanged(e);
        }

        void d_DownloadingChapter(object sender, DownloadingChapterEventArgs e) {
            if (d == null) return;
            Dispatcher.Invoke(() => {
                if (Format == null) Format = "{0}/{1}(失败{2}){3}";
                switch (e.State) {
                    case DownloadingChapterEventArgs.States.AllFail:
                    case DownloadingChapterEventArgs.States.Exception:
                        Text = "加载章节【" + e.StandardChapter.Title + "】失败";
                        ++Fail;
                        break;
                    case DownloadingChapterEventArgs.States.NonstandardSuccess:
                    case DownloadingChapterEventArgs.States.StandardSuccess:
                        Text = "加载章节【" + e.StandardChapter.Title + "】成功";
                        ++Success;
                        break;
                }
            });
        }

        void d_ContentFetched(object sender, EventArgs e) {
            if (d == null) return;
            if (d.StdSpider.BookDesc == null || d.StdSpider.Chapters == null) return;
            Text = String.Format(Format, Success, Total, Fail, "加载目录完成，正在加载章节");
            Format = null;
            try {
                Total = d.StdSpider.Chapters.Count;
                Success = Fail = 0;
            } catch(Exception ex) {
                Value = ex.Message;
            }
        }

        void d_Preparing(object sender, PreparingEventArgs e) {
            if (d == null) return;
            if (e.Exception == null) {
                Dispatcher.Invoke(() => {
                    Text = "从【" + e.Spider.Name + "】抓取目录成功";
                    ++Success;
                });
                
            } else {
                Dispatcher.Invoke(new Action(() => {
                    Text = "从【" + e.Spider.Name + "】抓取目录失败";
                    ++Fail;
                    if (e.Spider.IsStandard) {
                        MessageBox.Show(TXTReader.G.MainWindow,"下载小说\"" + e.Spider.BookDesc.Title + "\"失败");
                        Stop();
                        TXTReader.G.Book = null;
                    }                    
                }));                
            }
        }
    }
}
