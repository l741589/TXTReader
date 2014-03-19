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
using System.Collections.ObjectModel;

namespace TRBookmark {
    /// <summary>
    /// BookmarkPanel.xaml 的交互逻辑
    /// </summary>
    public partial class BookmarkPanel : UserControl {

        private ObservableCollection<Bookmark> Bookmarks {
            get {
                return lb_bookmark.ItemsSource as ObservableCollection<Bookmark>;
            }
        }

        private IContentAdapter Book {
            get {
                return TXTReader.G.Book as IContentAdapter;
            }
        }

        public BookmarkPanel() {
            InitializeComponent();
        }

        private void bn_add_Click(object sender, RoutedEventArgs e) {
            if (Bookmarks != null && Book.NotNull()) {
                Bookmark bmk = null;
                if (Book.CurrentChapter != null && Book.CurrentChapter is IPositionable) {
                    var chapter = Book.CurrentChapter;
                    bmk = new Bookmark(chapter as IPositionable);
                    bmk.Total = Book.TotalLineCount;
                    try {
                        if (bmk.Position < chapter.TotalText.Count) {
                            bmk.Preview = chapter.TotalText[bmk.Position];
                        }
                    } catch {
                        bmk.Preview = null;
                    }
                } else {
                    if (Book is IPositionable) {
                        bmk = new Bookmark(Book as IPositionable);
                        bmk.Total = Book.TotalLineCount;
                        try {
                            if (bmk.Position < Book.TotalText.Count) {
                                bmk.Preview = Book.TotalText[bmk.Position];
                            }
                        } catch {
                            bmk.Preview = null;
                        }
                    }
                }
                if (bmk == null) return;
                Bookmarks.Add(bmk);
            }
        }

        private void bn_del_Click(object sender, RoutedEventArgs e) {
            if (Bookmarks!=null && lb_bookmark.SelectedItems.Count > 0)
                Bookmarks.Remove(lb_bookmark.SelectedValue as Bookmark);
        }

        private void lb_book_DoubleClick(object sender, MouseButtonEventArgs e) {
            if (TXTReader.G.Book.IsNull()) return;
            var li = sender as ListBoxItem;
            var b = li.DataContext as Bookmark;            
            b.AssignTo(TXTReader.G.Book as IPositionable);
        }
    }
}
