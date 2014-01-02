using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TXTReader.Books;
using TXTReader.Widget;

namespace TXTReader.Rules {

    //不应该依赖界面而应该界面依赖这个
    class Rule : DependencyObject {
        public const String S_ADD = "<新增规则>";
        public const String S_NEW = "";
        public const String S_DBLNAME = "重复的文件名，请在第一行插入‘##’注释或者修改第一行来重新指定名称";

        private Trmex treeTrmex = null;
        private Trmex listTrmex = null;
        private bool listlock = false;
        private bool treelock = false;
        private ObservableCollection<String> list = new ObservableCollection<String>();
        private ObservableCollection<String> tree = new ObservableCollection<String>();

        public static readonly DependencyProperty IsListEnableProperty = DependencyProperty.Register("IsListEnable", typeof(bool), typeof(Rule), new PropertyMetadata(true));
        public static readonly DependencyProperty IsTreeEnableProperty = DependencyProperty.Register("IsTreeEnable", typeof(bool), typeof(Rule), new PropertyMetadata(false));
        public static readonly DependencyProperty ListTextProperty = DependencyProperty.Register("ListText", typeof(String), typeof(Rule));
        public static readonly DependencyProperty TreeTextProperty = DependencyProperty.Register("TreeText", typeof(ObservableCollection<String>), typeof(Rule));

        public String ListText { get { return (String)GetValue(ListTextProperty); } set { SetValue(ListTextProperty, value); } }
        public ObservableCollection<String> TreeText { get { return (ObservableCollection<String>)GetValue(TreeTextProperty); } set { SetValue(TreeTextProperty, value); } }
        public int ListSelectedIndex { get { return (int)GetValue(ListSelectedIndexProperty); } set { SetValue(ListSelectedIndexProperty, value); } }
        public int TreeSelectedIndex { get { return (int)GetValue(TreeSelectedIndexProperty); } set { SetValue(TreeSelectedIndexProperty, value); } }
        public bool IsListEnable { get { return (bool)GetValue(IsListEnableProperty); } set { SetValue(IsListEnableProperty, value); } }
        public bool IsTreeEnable { get { return (bool)GetValue(IsTreeEnableProperty); } set { SetValue(IsTreeEnableProperty, value); } }
        public Trmex ListTrmex { get { if (listTrmex != null) return listTrmex; else  return listTrmex = new Trmex(ListRule); } set { listTrmex = value; } }
        public Trmex TreeTrmex { get { if (treeTrmex != null) return treeTrmex; else  return treeTrmex = new Trmex(TreeRule); } set { treeTrmex = value; } }

        public Rule()
            : base() {
            ListText = S_NEW;
            TreeText = new ObservableCollection<string>();
            TreeText.Add(S_NEW);
        }

        public ObservableCollection<String> PrependAddItem(ObservableCollection<String> target) {
            if (target.Count > 0 && target[0] == S_ADD) return target;
            int i = target.IndexOf(S_ADD);
            if (i > 0) target.Move(i, 0);
            else target.Insert(0, S_ADD);
            return target;
        }

        public ObservableCollection<String> List { get { return PrependAddItem(list); } }
        public ObservableCollection<String> Tree { get { return PrependAddItem(tree); } }

        public String ListSelection { get { return List[ListSelectedIndex]; } set { ListSelectedIndex = List.IndexOf(value); } }
        public String TreeSelection { get { return Tree[TreeSelectedIndex]; } set { TreeSelectedIndex = Tree.IndexOf(value); } }

        public List<String> ListRule {
            get {
                String[] ret = ListText.Split('\n', '\r');
                return ret.ToList();
            }
            set {
                ListTrmex = null;
                if (value == null) {
                    ListText = "";
                } else {
                    ListText = String.Join("\n", value);
                }
            }
        }

        public List<List<String>> TreeRule {
            get {
                List<List<String>> formats = new List<List<String>>();
                foreach (var e in TreeText) {
                    String[] ss = e.Split('\n', '\r');
                    formats.Add(ss.ToList());
                }
                return formats;
            }
            set {
                TreeTrmex = null;
                if (TreeText == null) TreeText = new ObservableCollection<string>();
                int c = TreeText.Count;
                if (c <= 0) c = 1;
                TreeText.Clear();
                if (value == null) return;
                foreach (var e in value)
                    TreeText.Add(String.Join("\n", e));
                if (TreeText.Count == 0)
                    while (TreeText.Count < c)
                        TreeText.Add(S_NEW);
            }
        }

        public int LevelCount {
            get { return TreeText.Count; }
            set {
                int v = value;
                if (v < 1) v = 1;
                if (v > 10) v = 10;
                while (TreeText.Count < value) {
                    TreeText.Add(S_NEW);
                }
                while (TreeText.Count > value) TreeText.RemoveAt(TreeText.Count - 1);
            }
        }

        public static readonly DependencyProperty ListSelectedIndexProperty = DependencyProperty.Register("ListSelectedIndex", typeof(int), typeof(Rule), new PropertyMetadata(0, 
        (d, e) => {
            Rule o = d as Rule;
            if (o.listlock) return;
            o.listlock = true;
            try {
                if (!RuleParser.Load(o.ListSelection)) 
                    o.ListRule = null;
            } finally {
                o.listlock = false;
            }
        },
        (d, baseValue) => {
            Rule o = d as Rule;
            int i = (int)baseValue;
            if (i >= o.List.Count) i = o.List.Count - 1;
            if (i < 0) i = 0;
            return i;
        }));

        public static readonly DependencyProperty TreeSelectedIndexProperty = DependencyProperty.Register("TreeSelectedIndex", typeof(int), typeof(Rule), new PropertyMetadata(0,
        (d, e) => {
            Rule o = d as Rule;
            if (o.treelock) return;
            o.treelock = true;
            try {
                if (!RuleParser.Load(o.TreeSelection))
                    o.TreeRule = null;
            } finally {
                o.treelock = false;
            }
        },
        (d, baseValue) => {
            Rule o = d as Rule;
            int i = (int)baseValue;
            if (i >= o.Tree.Count) i = o.Tree.Count - 1;
            if (i < 0) i = 0;
            return i;
        }));

        public void ListTextChanged(object sender, TextChangedEventArgs e, String newValue) {
            ListTrmex = null;
            ListText = newValue;
            if (listlock) return;
            listlock = true;
            try {
               
                int i = 0;
                if (ListSelection != null && ListSelection != Rule.S_ADD) {
                    if (ListSelection != S_DBLNAME) File.Delete(ListSelection);
                    i = List.IndexOf(ListSelection);
                    List.Remove(ListSelection);
                }
                String name = RuleParser.SaveList();
                if (name != null) {
                    if (i == 0) List.Add(name);
                    else List.Insert(i, name);
                    ListSelection = name;
                }
            } catch (IOException) {
                List.Add(S_DBLNAME);
                ListSelection = S_DBLNAME;
            } finally {
                listlock = false;
            }
        }

        public void TreeTextChanged(object sender, TextChangedEventArgs e,String oldValue, String newValue) {
            TreeTrmex = null;
            int level = TreeText.IndexOf(oldValue);
            if (level != -1) TreeText[level] = newValue;
            if (treelock) return;
            treelock = true;
            try {                
                int i = 0;
                if (TreeSelection != null && TreeSelection != Rule.S_ADD) {
                    if (TreeSelection != S_DBLNAME) File.Delete(TreeSelection);
                    i = Tree.IndexOf(TreeSelection);
                    Tree.Remove(TreeSelection);
                }
                String name = RuleParser.SaveTree();
                if (name != null) {
                    if (i == 0) Tree.Add(name);
                    else Tree.Insert(i, name);
                    TreeSelection = name;
                }
            } catch (IOException) {
                Tree.Add(S_DBLNAME);
                TreeSelection = S_DBLNAME;
            } finally {
                treelock = false;
            }
        }


    }
}
