using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Zlib.Utility {
    public static class INotifyPropertyChangedExtension {

        private class C {
            Type st;
            Type tt;
            Dictionary<String, PropertyInfo> name1;
            Dictionary<String, PropertyInfo> name2;
            bool f;
            public INotifyPropertyChanged s;
            public object t;

            public void Bind(INotifyPropertyChanged s, object t) {
                if (s == null) return;
                if (t == null) return;
                this.s = s;
                this.t = t;
                st = s.GetType();
                tt = t.GetType();
                var pp = from sp in st.GetProperties().ToList()
                         from tp in tt.GetProperties().ToList()
                         where sp.Name == tp.Name && sp.CanRead && tp.CanWrite
                         select new { X = sp, Y = tp };
                name1 = pp.ToDictionary(i => i.X.Name, i => i.X);
                name2 = pp.ToDictionary(i => i.Y.Name, i => i.Y);
                f = false;

                s.PropertyChanged += s_PropertyChanged;
            }

            void s_PropertyChanged(object sender, PropertyChangedEventArgs e) {
                if (f) return;
                f = true;
                try {
                    if (!name1.ContainsKey(e.PropertyName)) return;
                    var sp = name1[e.PropertyName];
                    var tp = name2[e.PropertyName];
                    tp.SetValue(t, sp.GetValue(s));
                } finally {
                    f = false;
                }
            }

            public void Unbind() {
                s.PropertyChanged -= s_PropertyChanged;
            }
        }

        private static Dictionary<INotifyPropertyChanged, Dictionary<object, C>> dic = new Dictionary<INotifyPropertyChanged, Dictionary<object, C>>();

        public static void Unbind(this INotifyPropertyChanged s) {
            if (!dic.ContainsKey(s)) return;
            var x = dic[s];
            foreach (var y in x) {
                y.Value.Unbind();
                if (y.Value.t is INotifyPropertyChanged) {
                    (y.Value.t as INotifyPropertyChanged).Unbind((object)s);
                }
            }
        }

        public static void Unbind(this INotifyPropertyChanged s, object t) {
            if (!dic.ContainsKey(s)) return;
            var x = dic[s];
            if (!x.ContainsKey(t)) return;
            var c = x[t];
            c.Unbind();
            x.Remove(t);
            if (x.IsEmpty()) dic.Remove(s);
        }

        public static void Unbind(this INotifyPropertyChanged s, INotifyPropertyChanged t) {
            s.Unbind((object)t);
            t.Unbind((object)s);
        }

        public static void Bind(this INotifyPropertyChanged s, object t) {
            if (HasBinding(s, t)) return;
            var c = new C();
            Dictionary<object,C> x=null;
            if (dic.ContainsKey(s)) x = dic[s];
            else dic[s] = x = new Dictionary<object, C>();
            x[t] = c;
            c.Bind(s, t);
        }

        public static bool HasBinding(this INotifyPropertyChanged s, object t) {
            if (dic.ContainsKey(s)) {
                var x = dic[s];
                return x.Contains(i => i.Key == t);
            } else return false;
        }

        public static void Bind(this INotifyPropertyChanged s, INotifyPropertyChanged t) {
            Bind(s, (object)t);
            Bind(t, (object)s);
        }

        public static object[] GetBindingTarget(this INotifyPropertyChanged s) {
            if (!dic.ContainsKey(s)) return null;
            var x = dic[s];
            return (from i in x select i.Key).ToArray();
        }

        public static INotifyPropertyChanged[] GetBindingSource(this object t) {
            return (from i in dic where i.Value.Contains(j => j.Key == t) select i.Key).ToArray();
        }

        public static object GetValue(this PropertyInfo p,object obj) {
            return p.GetValue(obj, null);
        }

        public static void SetValue(this PropertyInfo p,object obj, object value) {
            p.SetValue(obj, value, null);
        }

        
    }
}
