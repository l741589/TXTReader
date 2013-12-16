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
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using TXTReader.Utility;
using TXTReader.Data;
using System.IO;

namespace TXTReader.Widget {
    /// <summary>
    /// ChapterRulePanel.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class RulePanel : UserControl {

        private readonly GridLength hiddenRow = new GridLength(0);
        private GridLength listRow;
        private GridLength treeRow;
        public RulePanel() {
            InitializeComponent();           
            listRow = panel.RowDefinitions[1].Height;
            treeRow = panel.RowDefinitions[4].Height;
            UpdateBinding();           
        }

        public void UpdateBinding() {
            DataContext = G.Rules;
            if (cbb_list.ItemsSource != G.Rules.List) cbb_list.ItemsSource = G.Rules.List;
            if (cbb_tree.ItemsSource != G.Rules.Tree) cbb_tree.ItemsSource = G.Rules.Tree;
            lb_list.SetBinding(TextBox.TextProperty, new Binding("ListText") { Mode = BindingMode.TwoWay });
            if (ic_tree.ItemsSource != G.Rules.TreeText) ic_tree.ItemsSource = G.Rules.TreeText;
            cb_list.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsListEnable") { Mode = BindingMode.TwoWay });
            cb_tree.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsTreeEnable") { Mode = BindingMode.TwoWay });
            cbb_list.SetBinding(ComboBox.SelectedIndexProperty, new Binding("ListSelectedIndex") { Mode = BindingMode.TwoWay });
            cbb_tree.SetBinding(ComboBox.SelectedIndexProperty, new Binding("TreeSelectedIndex") { Mode = BindingMode.TwoWay });            
        }

        private void bn_list_Click(object sender, RoutedEventArgs e) {
            if (panel.RowDefinitions.Count == 0) return;
            if ((sender as ToggleButton).IsChecked == false) {
                if (panel.RowDefinitions[1].Height != hiddenRow)
                    listRow = panel.RowDefinitions[1].Height;
                panel.RowDefinitions[1].Height = hiddenRow;
            } else {
                panel.RowDefinitions[1].Height = listRow;
            }
        }

        private void bn_tree_Click(object sender, RoutedEventArgs e) {
            if (panel.RowDefinitions.Count == 0) return;
            if ((sender as ToggleButton).IsChecked == false) {
                if (panel.RowDefinitions[4].Height != hiddenRow)
                    treeRow = panel.RowDefinitions[4].Height;
                panel.RowDefinitions[4].Height = hiddenRow;
            } else {
                panel.RowDefinitions[4].Height = treeRow;
            }
        }

        private void bn_add_Click(object sender, RoutedEventArgs e) {
            if (LevelCount >= 10) return;
            ++LevelCount;
            if (LevelCount > 1) bn_sub.IsEnabled = true;
            if (LevelCount >= 10) bn_add.IsEnabled = false;
        }

        private void bn_sub_Click(object sender, RoutedEventArgs e) {
            if (LevelCount <= 1) return;
            --LevelCount;
            if (LevelCount <= 1) bn_sub.IsEnabled = false;
            if (LevelCount < 10) bn_add.IsEnabled = true;
        }


        public int LevelCount { 
            get { return G.Rules.LevelCount; }
            set { G.Rules.LevelCount = value;}
        }


        public bool IsListFormatEnabled { get { return G.Rules.IsListEnable; } }
        public bool IsTreeFormatEnabled { get { return G.Rules.IsTreeEnable; } }

        void lb_list_TextChanged(object sender, TextChangedEventArgs e) {
            G.Rules.ListTextChanged(sender, e, lb_list.Text);
        }

        void lb_tree_TextChanged(object sender, TextChangedEventArgs e) {
            var tb=sender as TextBox;
            G.Rules.TreeTextChanged(sender, e, tb.DataContext.ToString(), tb.Text);
        }

        private void bn_dellist_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            String s = b.DataContext.ToString();
            File.Delete(s);
            G.Rules.List.Remove(s);
            var _ = G.Rules.ListSelection;
        }

        private void bn_deltree_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            String s = b.DataContext.ToString();
            File.Delete(s);
            G.Rules.Tree.Remove(s);
            var _ = G.Rules.TreeSelection;
        }
    }        
}
