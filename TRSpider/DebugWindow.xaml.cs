using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Shapes;
using TXTReader;
using Zlib.Utility;
using Zlib.Algorithm;
using Zlib.Text.ZSpiderScript;
using System.Diagnostics;

namespace TRSpider {
    /// <summary>
    /// DebugWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DebugWindow : Window {

        private String WorkDir { get { return A.CheckDir(G.PATH + @"spider\"); } }
        private ObservableCollection<CustomSpider> spiders = new ObservableCollection<CustomSpider>();

        private static DebugWindow instance;
        public static DebugWindow Instance {
            get {
                if (instance == null || !instance.IsVisible)
                    instance = new DebugWindow();
                return instance;
            }
        }

        private static Queue<String> StringToAdd = new Queue<String>();

        public static void Log(String text) {
            if (instance == null) return;
            lock (StringToAdd) {
                StringToAdd.Enqueue(text + "\r\n");
            }
        }

        public static void Log2(String text) {
            if (instance == null) return;
            lock (StringToAdd) {
                StringToAdd.Enqueue(text);
            }
        }

        public DebugWindow() {
            InitializeComponent();
            //Owner = G.MainWindow;
            cb_spider.ItemsSource = spiders;
            Load();
            Loaded += DebugWindow_Loaded;
            
        }

        void DebugWindow_Loaded(object sender, RoutedEventArgs e) {
            timer();
        }

        async void timer() {
            while (IsVisible) {
                lock (StringToAdd) {
                    foreach (var e in StringToAdd)
                        tb.AppendText(e);
                    StringToAdd.Clear();
                }
                await 200.Wait();
            }
        }

        public void Load() {
            var sel = cb_spider.SelectedIndex;
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
            if ((sel == -1 && cb_spider.Items.Count > 0) || sel >= cb_spider.Items.Count) cb_spider.SelectedIndex = 0;
            else cb_spider.SelectedIndex = sel;
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            //Load();
            if (cb_spider.SelectedItem == null) { MessageBox.Show("请选择一个爬虫"); return; }
            var cs = (cb_spider.SelectedItem as CustomSpider);
            try {
                BookDesc bd = null;
                pb.Visibility = Visibility.Visible;
                String text = tb_input.Text;
                tb.Text = "";
                var tt=TaskEx.Run(() => bd = cs.Search(text));
                await tt;
                if (tt.Exception != null) throw tt.Exception;
                if (bd != null) tb.Text += "\r\n---------------------------------\r\n" +
                      "封面:" + bd.CoverUrl + "\r\n" +
                      "书名:" + bd.Title + "\r\n" +
                      "作者:" + bd.Author + "\r\n" +
                      "ＩＤ:" + bd.Id + "\r\n" +
                      "首页:" + bd.EntryUrl + "\r\n" +
                      "目录:" + bd.ContentUrl + "\r\n" +
                      "描述:" + bd.Description;
            } catch (ZSSRuntimeException ee) {
                MessageBox.Show(ee.Message);
            } finally {
                pb.Visibility = Visibility.Collapsed;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e) {
            //Load();
            if (cb_spider.SelectedItem == null) { MessageBox.Show("请选择一个爬虫"); return; }
            var cs = (cb_spider.SelectedItem as CustomSpider); 
            try {
                pb.Visibility = Visibility.Visible;
                String text = tb_input.Text;
                var tt = TaskEx.Run(() => cs.GetContent(text));
                var bd = await tt;
                if (tt.Exception != null) throw tt.Exception.InnerException;
                lv.Items.Clear();
                foreach (var ee in bd)
                    lv.Items.Add(ee);
            } catch (ZSSRuntimeException ee) {
                MessageBox.Show(ee.Message);
            } finally {
                pb.Visibility = Visibility.Collapsed;
            }
        }

        private void sp_MouseMove(object sender, MouseEventArgs e) {
            if (sp.IsMouseCaptured) { 
                var p=e.GetPosition(lv);
                lv.Width = Math.Max(8, p.X - 2);
            }
        }

        private void sp_MouseDown(object sender, MouseButtonEventArgs e) {
            sp.CaptureMouse();
        }

        private void sp_MouseUp(object sender, MouseButtonEventArgs e) {
            sp.ReleaseMouseCapture();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            var cd = (sender as Control).DataContext as ChapterDesc;
            try {
                if (cd != null && !cd.TextUrl.IsNullOrWhiteSpace()) System.Diagnostics.Process.Start(cd.TextUrl);
                else MessageBox.Show("无效的链接");
            } catch { }
        }

        private async void MenuItem_Click_2(object sender, RoutedEventArgs e) {
            var cd = (sender as Control).DataContext as ChapterDesc;
            if (cb_spider.SelectedItem == null) { MessageBox.Show("请选择一个爬虫"); return; }
            var cs = (cb_spider.SelectedItem as CustomSpider);
            try {
                String bd = null;
                pb.Visibility = Visibility.Visible;
                await TaskEx.Run(() => bd = cs.GetText(cd.TextUrl));
                tb.Text = bd;
            } catch (ZSSRuntimeException ee) {
                MessageBox.Show(ee.Message);
            } finally {
                pb.Visibility = Visibility.Collapsed;
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e) {
            var cd = (sender as Control).DataContext as ChapterDesc;
            Clipboard.SetText("标题:" + cd.Title + "\r\n" + 
                "链接:" + cd.TextUrl + "\r\n" + 
                "ＩＤ:" + cd.Id);
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e) {
            var cd = (sender as Control).DataContext as ChapterDesc;
            Clipboard.SetText(cd.Title);            
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e) {
            var cd = (sender as Control).DataContext as ChapterDesc;
            Clipboard.SetText(cd.TextUrl);            
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e) {
            var cd = (sender as Control).DataContext as ChapterDesc;
            Clipboard.SetText(cd.Id);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            Load();
        }
    }
}