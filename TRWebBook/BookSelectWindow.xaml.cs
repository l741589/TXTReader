using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TRSpider;
using Zlib.Async;
using Zlib.Utility;

namespace TRWebBook {

    /// <summary>
    /// BookSelectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BookSelectWindow : Window {        


        private SpiderCollection Spiders = SpiderCollection.Instance;
        private ObservableCollection<SpiderClosure> items = new ObservableCollection<SpiderClosure>();
        private UniformGrid grid;
        private String text;
        public ObservableCollection<SpiderClosure> Items { get { return items; } }

        public BookSelectWindow(String text) {
            InitializeComponent();
            this.text = text;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            if (grid == null) return;
            grid.Columns = Math.Max(1, (int)ActualWidth / 256);
        }

        private void UniformGrid_Loaded(object sender, RoutedEventArgs e) {
            grid = sender as UniformGrid;
            Window_SizeChanged(this, null);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs args) {
            ZMultiTask zt = new ZMultiTask();
            foreach (var e in Spiders) {
                zt.Add(() => {
                    SpiderClosure ia = new SpiderClosure(e);
                    ia.Search(text);
                    if (ia.BookDesc.CoverUrl.IsNullOrWhiteSpace()) ia.BookDesc.CoverUrl = "/TRWebBook;component/res/no_cover.png";
                    if (ia.BookDesc.Description.IsNullOrWhiteSpace()) ia.BookDesc.Description = "<没有简介>";
                    Dispatcher.Invoke(() => { Items.Add(ia); });
                });
            }
            await zt.Execute();
            tb.Text = "搜索完成（双击书籍打开）";
            pb.Visibility = Visibility.Hidden;
        }

        private void Window_StateChanged(object sender, EventArgs e) {
            Window_SizeChanged(sender, null);
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var sp = (sender as FrameworkElement).DataContext as SpiderClosure;
            var b = new Book(new BookDownloader(Items, sp, 1));
            TXTReader.G.Book = b;
            Close();
        }
    }
}
