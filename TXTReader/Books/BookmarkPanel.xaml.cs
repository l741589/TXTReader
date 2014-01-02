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
using TXTReader.Utility;
using System.ComponentModel;

namespace TXTReader.Books {
    /// <summary>
    /// BookmarkPanel.xaml 的交互逻辑
    /// </summary>
    public partial class BookmarkPanel : UserControl {
        public BookmarkPanel() {
            InitializeComponent();
            lb_bookmark.ItemsSource = G.Bookmark;
            lb_bookmark.Items.IsLiveSorting = true;
            lb_bookmark.Items.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
        }

        private void bn_add_Click(object sender, RoutedEventArgs e) {
            if (G.Book!=null&&G.Book.Source!=null) G.Bookmark.Add(new Bookmark(G.Book));
        }

        private void bn_del_Click(object sender, RoutedEventArgs e) {
            if (lb_bookmark.SelectedItems.Count>0)
                G.Bookmark.Remove(lb_bookmark.SelectedItems[0] as Bookmark);
        }

        private void lb_book_DoubleClick(object sender, MouseButtonEventArgs e) {
            var li = sender as ListBoxItem;
            var b = li.DataContext as Bookmark;
            lb_bookmark.ItemsSource = G.Bookmark;
            b.AssignTo(G.Book);
            G.Displayer.Update();
        }
    }
}
