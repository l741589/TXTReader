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

namespace TXTReader.Widget {
    /// <summary>
    /// BookCasePanel.xaml 的交互逻辑
    /// </summary>
    public partial class BookcasePanel : UserControl {
        private static DependencyPropertyKey IsSettingMenuOpen = DependencyProperty.RegisterReadOnly("SettingMenuOpen", typeof(bool), typeof(BookcasePanel),new PropertyMetadata());

        public BookcasePanel() {
            InitializeComponent();
            lb_book.ItemsSource = G.Books;
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
                }
            }
        }

        private void DelBook() {
            if (G.Books.Count > 0)
                G.Books.Remove(lb_book.SelectedItems[0] as Book);
        }

        private void lbi_add_MouseUp(object sender, MouseButtonEventArgs e) { OpenBook(); }
        private void MenuItem_Click(object sender, RoutedEventArgs e) { OpenBook(); }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e) { DelBook(); }

        private async void lb_book_DoubleClick(object sender, MouseButtonEventArgs e) {
            var li = sender as ListBoxItem;
            var b = li.DataContext as Book;
            var i=lb_book.Items.IndexOf(b);
            lb_book.Items.RemoveAt(i);
            await b.Load();
            lb_book.Items.Insert(i, b);
            G.Displayer.OpenFile(b.Source);
        }
    }
}
