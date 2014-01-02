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
using System.IO;

namespace TXTReader.Rules {
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
            DataContext = G.Rule;
            if (cbb_list.ItemsSource != G.Rule.List) cbb_list.ItemsSource = G.Rule.List;
            if (cbb_tree.ItemsSource != G.Rule.Tree) cbb_tree.ItemsSource = G.Rule.Tree;
            lb_list.SetBinding(TextBox.TextProperty, new Binding("ListText") { Mode = BindingMode.TwoWay });
            if (ic_tree.ItemsSource != G.Rule.TreeText) ic_tree.ItemsSource = G.Rule.TreeText;
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
            get { return G.Rule.LevelCount; }
            set { G.Rule.LevelCount = value;}
        }


        public bool IsListFormatEnabled { get { return G.Rule.IsListEnable; } }
        public bool IsTreeFormatEnabled { get { return G.Rule.IsTreeEnable; } }

        void lb_list_TextChanged(object sender, TextChangedEventArgs e) {
            G.Rule.ListTextChanged(sender, e, lb_list.Text);
        }

        void lb_tree_TextChanged(object sender, TextChangedEventArgs e) {
            var tb=sender as TextBox;
            G.Rule.TreeTextChanged(sender, e, tb.DataContext.ToString(), tb.Text);
        }

        private void bn_dellist_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            String s = b.DataContext.ToString();
            File.Delete(s);
            G.Rule.List.Remove(s);
            var _ = G.Rule.ListSelection;
        }

        private void bn_deltree_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            String s = b.DataContext.ToString();
            File.Delete(s);
            G.Rule.Tree.Remove(s);
            var _ = G.Rule.TreeSelection;
        }
    }        
}
