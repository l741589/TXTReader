using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FloatControls {
    public class FloatControlCollection : ObservableCollection<IFloatControl> {

        private Dictionary<String, Visibility> dic = new Dictionary<String, Visibility>();
        private bool show;
        public bool Show {
            get { if (G.FloatControlOptionPanel != null) return G.FloatControlOptionPanel.header.IsChecked.Value; else return show; }
            set { if (G.FloatControlOptionPanel != null) G.FloatControlOptionPanel.header.IsChecked = value; else show = value; }
        }

        protected override void InsertItem(int index, IFloatControl item) {            
            if (item.Tag != null) {
                if (dic.ContainsKey(item.Tag.ToString())) item.Visibility = this[item.Tag.ToString()];
                else this[item.Tag.ToString()] = item.Visibility;
            }
            base.InsertItem(index, item);
        }

        public IFloatControl FindByTag(String tag) {
            foreach (IFloatControl e in this) if (e.Tag!=null&&e.Tag.ToString() == tag) return e;
            return null;
        }

        public Visibility this[String tag]{
            get {
                var f = FindByTag(tag);
                if (f != null) {
                    dic[tag] = f.Visibility;
                    return f.Visibility;
                }
                if (dic.ContainsKey(tag)) return dic[tag];
                return Visibility.Visible;
            }
            set {                
                dic[tag] = value;
                var f = FindByTag(tag);
                if (f != null) {
                    f.Visibility = value;
                }
            }
        }
    }
}
