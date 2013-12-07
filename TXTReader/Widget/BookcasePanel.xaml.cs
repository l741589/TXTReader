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
using TXTReader.Data;
using TXTReader.Utility;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;

namespace TXTReader.Widget {
    /// <summary>
    /// BookCasePanel.xaml 的交互逻辑
    /// </summary>
    public partial class BookcasePanel : UserControl {
        private static DependencyPropertyKey IsSettingMenuOpen = DependencyProperty.RegisterReadOnly("SettingMenuOpen", typeof(bool), typeof(BookcasePanel),new PropertyMetadata());

        public BookcasePanel() {
            InitializeComponent();
            lb_book.ItemsSource = G.Books;
            lb_book.Items.IsLiveSorting = true;
            lb_book.Items.SortDescriptions.Add(new SortDescription("SortArgument", ListSortDirection.Descending));  
            lb_book.Items.SortDescriptions.Add(new SortDescription("LastLoadTime", ListSortDirection.Descending));
            //G.Books.CollectionChanged += Books_CollectionChanged;
            //G.Books.CollectionChanged += (a, e) => {
            //    foreach (var b in e.NewItems) 
            //        (b as Book).SortArgument = A.FuzzyMatch((b as Book).Title, tb_search.Text, 10000);
            //};
        }

        private void Books_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems == null) return;
            foreach (var b in e.NewItems)
                (b as Book).SortArgument = A.FuzzyMatch((b as Book).Title, tb_search.Text, 10000);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            /*Book b = new Book();
            b.Load(@"E:\GitHub\TXTReader\TXTReader\测试.txt", G.Trmex);
            b.Author = "asfddg";
            b.Cover = (ImageSource)new ImageSourceConverter().ConvertFrom("E:/test/IMG3.png");
            b.Position = 6577;
            lb_book.Items.Add(b);
            lb_book.Items.Add(b);*/            
        }


        public Boolean IsBookExists(String src) {
            foreach (var b in lb_book.Items) {
                if ((b as Book).Source == src) return true;
            }
            return false;
        }

        private void OpenBook(){
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.DefaultExt = "txt";
            dlg.Filter = Properties.Resources.FILE_FILTER;
            if (dlg.ShowDialog() == true) {
                foreach (var f in dlg.FileNames) {
                    if (IsBookExists(f)) continue;
                    Book b = new Book(f);
                    G.Books.Add(b);
                    BookParser.Save(b);
                }
            }
        }

        private void DelBook() {
            if (G.Books.Count > 0 && lb_book.SelectedItems != null && lb_book.SelectedItems.Count > 0)
                G.Books.Remove(lb_book.SelectedItems[0] as Book);
        }

        private void lbi_add_MouseUp(object sender, MouseButtonEventArgs e) { OpenBook(); }
        private void MenuItem_Click(object sender, RoutedEventArgs e) { OpenBook(); }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e) { DelBook(); }

        private void lb_book_DoubleClick(object sender, MouseButtonEventArgs e) {
            var li = sender as ListBoxItem;
            var b = li.DataContext as Book;
            
            G.Displayer.OpenBook(b);
            var i=G.Books.IndexOf(b);
            G.Books.Remove(b);
            G.Books.Add(b);
            //A.Resort(lb_book);
            lb_book.ScrollIntoView(b);
        }

        private void tb_search_KeyUp(object sender, KeyEventArgs e) { Search(); }
        private void tb_search_TextChanged(object sender, TextChangedEventArgs e) { Search(); }

        private void Search(){
            foreach (var b in G.Books)
                b.SortArgument = A.FuzzyMatch(b.Title, tb_search.Text, 1000) + A.FuzzyMatch(b.Author, tb_search.Text, 1000);
            A.Resort(lb_book);
            lb_book.ScrollIntoView(lb_book.Items[0]);
        }

        private void lb_book_KeyUp(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Delete: DelBook(); break;
                case Key.Insert: OpenBook(); break;
            }
        }
    }
}
