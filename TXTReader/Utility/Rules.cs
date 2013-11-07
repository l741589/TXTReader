using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTReader.Widget;

namespace TXTReader.Utility {
    class Rules {
        public const String S_ADD = "<新增规则>";

        private ObservableCollection<String> list = new ObservableCollection<String>();
        private ObservableCollection<String> tree = new ObservableCollection<String>();
       
        public RulePanel RulePanel { get { return G.MainWindow.toolPanel.pn_rule; } }

        public ObservableCollection<String> PrependAddItem(ObservableCollection<String> target) {
            if (target.Count > 0 && target[0] == S_ADD) return target;
            int i = target.IndexOf(S_ADD);
            if (i > 0) target.Move(i, 0);
            else target.Insert(0,S_ADD);
            return target;
        }

        public ObservableCollection<String> List { get { return PrependAddItem(list); } }
        public ObservableCollection<String> Tree { get { return PrependAddItem(tree); } }

        public List<String> CurrentList {
            get { return RulePanel.ListRule; }
            set { RulePanel.ListRule = value; }
        }
        public List<List<String>> CurrentTree {
            get { return RulePanel.TreeRule; }
            set { RulePanel.TreeRule = value; }
        }
        public String ListSelection {
            get {
                if (RulePanel.cbb_list.SelectedItem == null)
                    RulePanel.cbb_list.SelectedIndex = 0;
                return RulePanel.cbb_list.SelectedItem.ToString();
            }
            set {
                RulePanel.cbb_list.SelectedItem = value;
                if (RulePanel.cbb_list.SelectedItem == null)
                    RulePanel.cbb_list.SelectedIndex = 0;
            }
        }
        public String TreeSelection {
            get {
                if (RulePanel.cbb_tree.SelectedItem == null)
                    RulePanel.cbb_tree.SelectedIndex = 0;
                return RulePanel.cbb_tree.SelectedItem.ToString();
            }
            set {
                RulePanel.cbb_tree.SelectedItem = value;
                if (RulePanel.cbb_tree.SelectedItem==null)
                    RulePanel.cbb_tree.SelectedIndex = 0;
            }
        }
    }
}
