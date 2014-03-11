using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TXTReader;
using Zlib.Algorithm;
using System.IO;
using System.Collections.ObjectModel;
using Zlib.Text.ZSpiderScript;
using Zlib.Utility;
using System.Diagnostics;

namespace TRSpider {
    

    /// <summary>
    /// SpiderPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SpiderPanel : UserControl {

        private String WorkDir { get { return A.CheckDir(G.PATH + @"spider\"); } }
        private SpiderCollection spiders = SpiderCollection.Instance;
        private GridLength GL_U0 = new GridLength(0);
        private GridLength GL_U1 = new GridLength(1.618, GridUnitType.Star);

        public SpiderPanel() {
            InitializeComponent();
            lv.ItemsSource = spiders;
            Load();
        }

        public void Load() {
        #if DEBUG
            tb.Text = "星辰变";
        #endif
            spiders.Clear();
            String[] files = Directory.GetFiles(WorkDir, "*.zss");
            foreach (var f in files) {
                try {
                    CustomSpider cs = TRZSS.Instance.Load(f);
                    spiders.Add(cs);
                } catch (ZSSParseException ee) {
                    MessageBox.Show("Exception Occurs When Parsing '" + f + "':\n" + ee.Message);
                }
            }
            spiders.OnLoaded(spiders, EventArgs.Empty);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            var cs = ((sender as Control).DataContext as CustomSpider);
            try {
                var bd = cs.Search(tb.Text);
                MessageBox.Show(bd.CoverUrl + "\r\n"
                    + bd.Title + "\r\n" + bd.Author+"\r\n"
                    + bd.Id + "\r\n" + bd.EntryUrl + "\r\n" + bd.ContentUrl);
            } catch (ZSSRuntimeException ee) {
                MessageBox.Show(ee.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Load();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) {
            var cs = ((sender as Control).DataContext as CustomSpider);
            try {
                var bd = cs.GetContent(tb.Text);
                //foreach (var ee in bd)
                    //DisplayWindow.Add(ee.TextUrl + "\t" + ee.Id + "\t" + ee.Title);
            } catch (ZSSRuntimeException ee) {
                MessageBox.Show(ee.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            DebugWindow.Instance.Show();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs ee) {
            lv_book.Items.Clear();
            String text = tb.Text;
            List<Task> tasks = new List<Task>();
            foreach (var e in spiders) {
                tasks.Add(TaskEx.Run(() => {
                    try {
                        var bd = e.Search(text);
                        SpiderClosure bia = new SpiderClosure(e);
                        bia.BookDesc = bd;
                        Dispatcher.Invoke(() => lv_book.Items.Add(bia));
                    } catch { }
                }));
            }
            pb.Visibility = Visibility.Visible;
            try { await TaskEx.WhenAll(tasks); } catch { } finally {
                pb.Visibility = Visibility.Hidden;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                var bia = (sender as FrameworkElement).DataContext as SpiderClosure;
                new DownloadWindow(lv_book.Items, bia).Show();
                //var d=new BookDownloader(lv_book.Items, bia);
                //var s = await d.Download(bia.BookDesc.Title);
                //if (s.IsNullOrWhiteSpace()) {
                //    MessageBox.Show("下载失败");
                //    return;
                //}
                //File.WriteAllText("e:/test/ouf.txt", s, Encoding.UTF8);
            }
        }

        
    }
}
