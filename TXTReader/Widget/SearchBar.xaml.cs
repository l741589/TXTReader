using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using TXTReader.Data;
using TXTReader.Utility;

namespace TXTReader.Widget {
    /// <summary>
    /// SearchBar.xaml 的交互逻辑
    /// </summary>
    public partial class SearchBar : UserControl {

        private Regex regex;
        private Trmex trmex;
        private ObservableCollection<String> buffer = new ObservableCollection<String>();
        public SearchBar() {
            InitializeComponent();
            cb.ItemsSource = buffer;
            this.PreviewKeyDown += SearchBar_PreviewKeyDown;
        }

        void SearchBar_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                G.MainWindow.ReleaseHold(MainWindow.HC_FIND);
                e.Handled = true;
            }
        }

        public void Show() {
            if (!IsVisible) Visibility = Visibility.Visible;
            cb.Focus();
        }

        public void Hide() {
            if (IsVisible) Visibility = Visibility.Hidden;
        }

        private void cb_Checked(object sender, RoutedEventArgs e) {
            if (cb_regex == null || cb_circute == null || cb_trmex == null || cb_wholeword == null || cb_casesensetive == null) return;
            trmex = null;
            regex = null;
            if (cb_regex.IsChecked == true) {
                cb_casesensetive.IsEnabled = false;
                cb_trmex.IsEnabled = false;
                cb_wholeword.IsEnabled = false;
            } else if (cb_trmex.IsChecked == true) {
                cb_casesensetive.IsEnabled = false;
                cb_regex.IsEnabled = false;
                cb_wholeword.IsEnabled = false;
            } else {
                cb_regex.IsEnabled = true;
                cb_trmex.IsEnabled = true;
                cb_wholeword.IsEnabled = true;
                cb_casesensetive.IsEnabled = true;
                cb_circute.IsEnabled = true;
            }
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

        private bool RegexCmp(String s) {
            if (regex==null||regex.ToString()!=cb.Text) regex=new Regex(cb.Text);
            return regex.IsMatch(s);
        }

        private bool TrmexCmp(String s) {
            if (trmex == null || trmex.ToString() != cb.Text) trmex = new Trmex(cb.Text);
            return trmex.IsMatch(s);
        }

        private bool NormalCmp(String s) {
            String t = cb.Text;
            if (cb_casesensetive.IsChecked!=true) {
                t = t.ToLower();
                s = s.ToLower();
            }
            if (cb_wholeword.IsChecked == true) {
                t = "\\b" + Regex.Escape(t) + "\\b";
                if (regex == null || regex.ToString() != cb.Text) regex = new Regex(t);
                return regex.IsMatch(s);
            } else {
                return s.Contains(t);
            }
        }

        private bool Cmp(String s) {
            if (cb_regex.IsChecked == true) return RegexCmp(s);
            if (cb_trmex.IsChecked == true) return TrmexCmp(s);
            return NormalCmp(s);
        }

        public void Search(bool forward = true){
            if (cb.Text == null || cb.Text == "") return;
            if (!buffer.Contains(cb.Text)) buffer.Add(cb.Text);
            var texts = G.Displayer.Text;
            if (texts == null) return;
            int i = G.Displayer.FirstLine;
            do {
                if (forward) ++i; else --i;
                if (cb_circute.IsChecked==true){
                    if (i<0) i=texts.Length-1;
                    else if (i>=texts.Length) i=0;
                }else{
                    if (i < 0 || i >= texts.Length) {
                        MessageBox.Show("搜索完毕，没有找到需要的结果");
                        return;
                    }
                }
                if (Cmp(texts[i])) {
                    G.Displayer.FirstLine = i;
                    G.Displayer.Offset = 0;
                    G.Displayer.Update();
                    return;
                }
            } while (i != G.Displayer.FirstLine);
        }

        private void next_Click(object sender, RoutedEventArgs e) {
            previous.IsDefault = false;
            next.IsDefault = true;
            Search();
        }

        private void previous_Click(object sender, RoutedEventArgs e) {
            next.IsDefault = false;
            previous.IsDefault = true;
            Search(false);
        }
    }
}
