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
        private bool listlock = false;
        private bool treelock = false;
        private const String S_DBLNAME = "重复的文件名，请使在第一行插入‘##’注释或者修改第一行来重新指定名称";
        public RulePanel() {
            InitializeComponent();
            listRow = panel.RowDefinitions[1].Height;
            treeRow = panel.RowDefinitions[4].Height;
            cbb_list.ItemsSource = G.Rules.List;
            cbb_tree.ItemsSource = G.Rules.Tree;
            
            Loaded += RulePanel_Loaded;
            lb_list.TextChanged += lb_list_TextChanged;
            foreach (var tb in gp_tree.Children)
                if (tb is TextBox)
                    (tb as TextBox).TextChanged += lb_tree_TextChanged;
        }

        

        void RulePanel_Loaded(object sender, RoutedEventArgs e) {
            cbb_list.SelectedIndex = 0;
            cbb_tree.SelectedIndex = 0;
            RuleParser.Load();
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
            get { return gp_tree.Children.Count; }
            set {
                int v=value;
                if (v<1) v=1;
                if (v>10) v=10;
                while (gp_tree.Children.Count < value) {
                    var tb = new TextBox();
                    tb.TextChanged+=lb_tree_TextChanged;
                    gp_tree.Children.Add(tb);
                }
                while (gp_tree.Children.Count > value) gp_tree.Children.RemoveAt(gp_tree.Children.Count - 1);
            }
        }

        public List<String> ListRule {
            get {
                String[] ret = lb_list.Text.Split('\n', '\r');
                //Debug.Assert(ret.Count() == lb_list.LineCount);
                return ret.ToList();
            }
            set {
                lb_list.Clear();
                if (value == null) return;
                foreach (var s in value)
                    lb_list.AppendText(s + "\n");
            }
        }

        public List<List<String>> TreeRule {
            get {
                List<List<String>> formats = new List<List<String>>();
                foreach (var e in gp_tree.Children) {
                    if (e is TextBox) {
                        TextBox tb = e as TextBox;
                        String[] ss = tb.Text.Split('\n', '\r');
                        //Debug.Assert(ss.Count() == tb.LineCount);
                        formats.Add(ss.ToList());
                    }
                }
                return formats;
            }
            set {
                if (value != null) {
                    LevelCount = value.Count;
                    int i = 0;
                    foreach (var e in gp_tree.Children) {
                        if (e is TextBox) {
                            TextBox tb = e as TextBox;
                            tb.Text = String.Join("\n", value[i++]);
                        }
                    }
                } else {
                    foreach (var e in gp_tree.Children) {
                        if (e is TextBox) {
                            TextBox tb = e as TextBox;
                            tb.Clear();
                        }
                    }
                }
            }
        }
        public bool IsListFormatEnabled { get { return cb_list.IsChecked == true; } }
        public bool IsTreeFormatEnabled { get { return cb_tree.IsChecked == true; } }

        private void cbb_list_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (listlock) return;
            if (e.RemovedItems.Contains(S_DBLNAME)) {
                MessageBox.Show(S_DBLNAME);
                G.Rules.ListSelection = S_DBLNAME;
                return;
            }
            listlock = true;
            if (e.AddedItems.Count > 0)
                if (!RuleParser.Load(e.AddedItems[0].ToString()))
                    ListRule = null;
            listlock = false;
        }

        private void cbb_tree_SelectionChanged(object sender, SelectionChangedEventArgs e) {            
            if (treelock) return;
            if (e.RemovedItems.Contains(S_DBLNAME)) {
                MessageBox.Show(S_DBLNAME);
                G.Rules.TreeSelection = S_DBLNAME;
                return;
            }
            treelock = true;
            try {
                if (e.AddedItems.Count > 0)
                    if (!RuleParser.Load(e.AddedItems[0].ToString()))
                        TreeRule = null;
            } finally {
                treelock = false;
            }
        }

        void lb_list_TextChanged(object sender, TextChangedEventArgs e) {
            if (listlock) return;
            listlock = true;
            try {
                int i = 0;
                if (G.Rules.ListSelection != null && G.Rules.ListSelection != Rules.S_ADD) {
                    if (G.Rules.ListSelection != S_DBLNAME) File.Delete(G.Rules.ListSelection);
                    i = G.Rules.List.IndexOf(G.Rules.ListSelection);
                    G.Rules.List.Remove(G.Rules.ListSelection);
                }
                String name = RuleParser.SaveList();
                if (name != null) {
                    if (i == 0) G.Rules.List.Add(name);
                    else G.Rules.List.Insert(i, name);
                    G.Rules.ListSelection = name;
                }
            } catch (IOException) {
                G.Rules.List.Add(S_DBLNAME);
                G.Rules.ListSelection = S_DBLNAME;
            } finally {
                listlock = false;
            }
        }

        void lb_tree_TextChanged(object sender, TextChangedEventArgs e) {
            if (treelock) return;
            treelock = true;
            try {
                int i = 0;
                if (G.Rules.TreeSelection != null && G.Rules.TreeSelection != Rules.S_ADD) {
                    if (G.Rules.TreeSelection!=S_DBLNAME) File.Delete(G.Rules.TreeSelection);
                    i = G.Rules.Tree.IndexOf(G.Rules.TreeSelection);
                    G.Rules.Tree.Remove(G.Rules.TreeSelection);
                }
                String name = RuleParser.SaveTree();
                if (name != null) {
                    if (i == 0) G.Rules.Tree.Add(name);
                    else G.Rules.Tree.Insert(i, name);
                    G.Rules.TreeSelection = name;
                }
            } catch (IOException) {
                G.Rules.Tree.Add(S_DBLNAME);
                G.Rules.TreeSelection = S_DBLNAME;
            } finally {
                treelock = false;
            }
        }

        private void bn_dellist_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            String s = b.DataContext.ToString();
            File.Delete(s);
            G.Rules.List.Remove(s);
        }

        private void bn_deltree_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            String s = b.DataContext.ToString();
            File.Delete(s);
            G.Rules.Tree.Remove(s);
        }
    }        
}
