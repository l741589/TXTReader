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
using System.ComponentModel;
using Zlib.Utility;
using TRContent;

namespace TRBook {
    /// <summary>
    /// BookmarkPanel.xaml 的交互逻辑
    /// </summary>
    public partial class BookmarkPanel : UserControl {
        public BookmarkPanel() {
            InitializeComponent();
            if ((G.Book as Book).NotNull())
                lb_bookmark.ItemsSource = (G.Book as Book).Bookmark;
#if NET45
            lb_bookmark.Items.IsLiveSorting = true;
#endif
            lb_bookmark.Items.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
            G.BookChanged += (d, e) => { 
                if ((G.Book as Book).NotNull() && lb_bookmark.ItemsSource != (G.Book as Book).Bookmark) 
                    lb_bookmark.ItemsSource = (G.Book as Book).Bookmark; 
            };
        }

        private void bn_add_Click(object sender, RoutedEventArgs e) {
            if ((G.Book as Book).NotNull() && (G.Book as Book).Source != null) 
                (G.Book as Book).Bookmark.Add(new Bookmark(G.Book as Book));
        }

        private void bn_del_Click(object sender, RoutedEventArgs e) {
            if ((G.Book as Book).NotNull() && lb_bookmark.SelectedItems.Count > 0)
                (G.Book as Book).Bookmark.Remove(lb_bookmark.SelectedItems[0] as Bookmark);
        }

        private void lb_book_DoubleClick(object sender, MouseButtonEventArgs e) {
            var li = sender as ListBoxItem;
            var b = li.DataContext as Bookmark;
            lb_bookmark.ItemsSource = (G.Book as Book).Bookmark;
            b.AssignTo(G.Book as Book);
            G.Displayer.Update();
        }
    }
}
