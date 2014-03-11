using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Net {
    public class UrlArgs {
        private Dictionary<String, String> args;

        public UrlArgs() {
        }

        public UrlArgs(String key, object value) {
            args = new Dictionary<String, String>();
            args.Add(key, value.ToString());
        }

        public UrlArgs(UrlArgs l, UrlArgs r) {
            if (l == null) args = r.args;
            else if (r == null) args = l.args;
            else args = l.args.Union(r.args).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public override string ToString() {
            if (args == null) return "";
            String s = "";
            Boolean start = true;
            Dictionary<String, String>.KeyCollection keys = args.Keys;
            foreach (String key in keys) {
                if (start) start = false;
                else s += "&";
                s += key + "=" + args[key];
            }
            return s;
        }

        public static UrlArgs operator&(UrlArgs l,UrlArgs r){
            if (l == null || l.args == null) return r;
            if (r == null || r.args == null) return l;
            var result = new UrlArgs(l , r);
            return result;
        }

        public static implicit operator String(UrlArgs a) {
            if (a == null) return null;
            return a.ToString();
        }
    }
}
