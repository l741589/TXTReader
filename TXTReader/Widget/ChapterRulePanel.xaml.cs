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

namespace TXTReader.Widget {
    /// <summary>
    /// ChapterRulePanel.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class ChapterRulePanel : UserControl {

        private readonly GridLength hiddenRow = new GridLength(0);
        private GridLength listRow;
        private GridLength treeRow;

        public ChapterRulePanel() {
            InitializeComponent();
            listRow = panel.RowDefinitions[1].Height;
            treeRow = panel.RowDefinitions[4].Height;
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
            gp_tree.Children.Add(new TextBox());
            if (gp_tree.Children.Count > 1) bn_sub.IsEnabled = true;
        }

        private void bn_sub_Click(object sender, RoutedEventArgs e) {
            if (gp_tree.Children.Count <= 1) return;
            gp_tree.Children.RemoveAt(gp_tree.Children.Count - 1);
            if (gp_tree.Children.Count <= 1) bn_sub.IsEnabled = false;
        }

        public String[] Format {
            get {
                String[] ret = lb_list.Text.Split('\n', '\r');
                Debug.Assert(ret.Count() == lb_list.LineCount);
                return ret;
            }
        }

        public List<String[]> Formats {
            get {
                List<String[]> formats = new List<String[]>();
                foreach (var e in gp_tree.Children) {
                    if (e is TextBox) {
                        TextBox tb = e as TextBox;
                        String[] ss = tb.Text.Split('\n', '\r');
                        Debug.Assert(ss.Count() == tb.LineCount);
                        formats.Add(ss);
                    }
                }
                return formats;
            }

        }
        public bool IsListFormatEnabled { get { return cb_list.IsChecked == true; } }
        public bool IsTreeFormatEnabled { get { return cb_tree.IsChecked == true; } }
    }        
}
