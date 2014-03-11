using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using TXTReader;
using Zlib.Utility;

namespace TRSearchBar {

    /// <summary>
    /// SearchBar.xaml 的交互逻辑
    /// </summary>
    public partial class SearchBar : UserControl {

        

        private ObservableCollection<String> buffer = new ObservableCollection<String>();
        private SearchBarComparerCollection cmps = SearchBarComparerCollection.Instance;
        private ISearchBarComparer cmp = null;
        private TextBox tb;
        public SearchBar() {
            InitializeComponent();
            cb.ItemsSource = buffer;
            ic_cmps.ItemsSource = cmps;
            cmps.Insert(0, cmp=new NormalComparer());
            cmps.Insert(1, new RegexComparer());
            G.MainWindow.MouseDown += MainWindow_MouseDown;
            G.MainWindow.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, find_Executed, find_CanExecute));
            KeyDown += SearchBar_KeyDown;            
            Loaded += SearchBar_Loaded;

        }

        void SearchBar_Loaded(object sender, RoutedEventArgs e) {
            tb = cb.Template.FindName("PART_EditableTextBox", cb) as TextBox;
            Hide();
        }
        
        void SearchBar_KeyDown(object sender, KeyEventArgs e) {
            if (!IsVisible) return;
            if (e.Key == Key.Escape) {
                Hide();
                e.Handled = true;
            }
        }

        private void find_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = G.Book.NotNull(); }
        private void find_Executed(object sender, ExecutedRoutedEventArgs e) { Show(); }

        void MainWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (this.IsAncestorOf(e.OriginalSource as DependencyObject)) return;
            Visibility = Visibility.Collapsed;
        }

        public void Show() {
            if (!IsVisible) Visibility = Visibility.Visible;  
            tb.Focus();
        }

        public void Hide() {
            if (IsVisible) Visibility = Visibility.Hidden;
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

        private bool Cmp(String s) {
            if (cmp == null) return false;
            return cmp.Cmp(s, cb.Text, new CmpOption { 
                CaseSensitive=cb_casesensetive.IsChecked==true,
                Circuit=cb_circuit.IsChecked==true,
                WholeWord=cb_wholeword.IsChecked==true
            });
        }

        public void Search(bool forward = true){
            if (cb.Text == null || cb.Text == "") return;
            if (!buffer.Contains(cb.Text)) buffer.Add(cb.Text);
            var texts = G.Book.TotalText;
            if (texts == null) return;
            int i = G.Book.Position;
            do {
                if (forward) ++i; else --i;
                if (cb_circuit.IsChecked==true){
                    if (i<0) i=texts.Count-1;
                    else if (i >= texts.Count) i = 0;
                }else{
                    if (i < 0 || i >= texts.Count) {
                        MessageBox.Show("搜索完毕，没有找到需要的结果");
                        return;
                    }
                }
                if (Cmp(texts[i])) {
                    G.Book.Position = i;
                    G.Book.Offset = 0;
                    return;
                }
            } while (i != G.Book.Position);
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            cmp = (sender as Control).DataContext as ISearchBarComparer;
        }

        private bool isChecked = false;
        private void RadioButton_Loaded(object sender, RoutedEventArgs e) {
            if (isChecked) return;
            (sender as ToggleButton).IsChecked = isChecked = true;
        }
    }
}
