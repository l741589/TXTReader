using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Zlib.Algorithm;
using Zlib.Utility;
using TXTReader.Interfaces;
using TXTReader.Plugins;
using System.IO;

namespace TRBookcase {
    /// <summary>
    /// BookCasePanel.xaml 的交互逻辑
    /// </summary>
    public partial class BookcasePanel : UserControl {
        private static DependencyPropertyKey IsSettingMenuOpen = DependencyProperty.RegisterReadOnly("SettingMenuOpen", typeof(bool), typeof(BookcasePanel), new PropertyMetadata());

        public BookcasePanel() {
            InitializeComponent();
            lb_book.ItemsSource = G.Books;
            lb_book.Items.SortDescriptions.Add(new SortDescription("SortArgument", ListSortDirection.Descending));
            lb_book.Items.SortDescriptions.Add(new SortDescription("LastLoadTime", ListSortDirection.Descending));                 
        }

        private void Books_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems == null) return;
            foreach (var b in e.NewItems)
                (b as IBook).SortArgument = StringCompare.LongestCommonSubsequenceLength((b as IBook).Title, tb_search.Text);
        }

        private void bn_setting_MouseEnter(object sender, MouseEventArgs e) {
            pp_setting.IsOpen = true;
        }

        private void bn_setting_MouseLeave(object sender, MouseEventArgs e) {
            if (pp_setting.IsMouseOver) return;
            pp_setting.IsOpen = false;
        }

        private void pp_setting_MouseLeave(object sender, MouseEventArgs e) {
            if (bn_setting.IsMouseOver) return;
            pp_setting.IsOpen = false;
        }

        private void pp_setting_MouseUp(object sender, MouseButtonEventArgs e) {
            pp_setting.IsOpen = false;
        }

        public Boolean IsBookExists(String src, String id = null) {
            foreach (var b in lb_book.Items) {
                String s=(b as IBook).Source;
                if ( s == src) return true;
                if (BookParser.GetBookPath(s) == src) return true;
                if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty((b as IBook).Id)) {
                    if (id == (b as IBook).Id) return true;
                }
            }
            return false;
        }

        private void OpenBook() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.DefaultExt = "trb";
            dlg.Filter = PluginManager.Instance.OpenFilter;
            if (dlg.ShowDialog() == true) {
                foreach (var f in dlg.FileNames) {
                    if (System.IO.Path.GetExtension(f) == ".trb") {
                        if (IsBookExists(f)) continue;
                        var b = new BookCaseItem();
                        BookParser.Load(f, b);
                        if (IsBookExists(f, b.Id)) continue;
                        G.Books.Add(b);
                    } else {
                        var b = new BookCaseItem(f);
                        b.IsLoaded = true;
                        BookParser.Save(b);
                        G.Books.Add(b);
                    }
                }
            }
        }

        private void DelBook() {
            if (G.Books.Count > 0 && lb_book.SelectedItems != null && lb_book.SelectedItems.Count > 0) {
                IBook b = lb_book.SelectedItems[0] as IBook;
                G.Books.Remove(b);
                try { File.Delete(BookParser.GetBookPath(b)); } catch { }
            }
        }

        private void lbi_add_MouseUp(object sender, MouseButtonEventArgs e) { OpenBook(); }
        private void MenuItem_Click(object sender, RoutedEventArgs e) { OpenBook(); }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e) { DelBook(); }

        private void lb_book_DoubleClick(object sender, MouseButtonEventArgs e) {
            var li = sender as ListBoxItem;
            var b = li.DataContext as IBook;
            b.Load();
            var i = G.Books.IndexOf(b);
            G.Books.Remove(b);
            G.Books.Add(b);
            lb_book.ScrollIntoView(b);
        }

        private void tb_search_KeyUp(object sender, KeyEventArgs e) { Search(); }
        private void tb_search_TextChanged(object sender, TextChangedEventArgs e) { Search(); }

        private void Search() {
            foreach (var b in G.Books)
                b.SortArgument = A.FuzzyMatch(b.Title, tb_search.Text, 1000) + A.FuzzyMatch(b.Author, tb_search.Text, 1000);
            if (tb_search.Text == null || tb_search.Text == "") {
                lb_book.Items.Filter = null;
            } else {
                lb_book.Items.Filter = b => (b as IBook).SortArgument > 0;
                if (lb_book.Items.Count > 0)
                    lb_book.ScrollIntoView(lb_book.Items[0]);
            }
            lb_book.Items.Refresh();
        }

        private void lb_book_KeyUp(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Delete: DelBook(); break;
                case Key.Insert: OpenBook(); break;
            }
        }

        private void download_Click(object sender, RoutedEventArgs e) {
            var li = sender as Button;
            var b = li.DataContext as IBook;
            //await b.Download();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e) {
            //var li = sender as Control;
            //var b = li.DataContext as Book;
            //if (b.State == Book.BookState.Local)
            //    await b.Upload();
        }
    }
}
