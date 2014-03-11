using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Zlib.Text.ZSpiderScript {
    class JsonDecodeCommand :ZSSCommand{

        public override string Command {
            get { return "jsondecode"; }
        }

        public JsonDecodeCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
        }

        public override string DoExecute(string input) {
            Object obj = Json.Decode(this[0] == null ? input : GenVar(this[0]));

            if (obj is Dictionary<String, Object>) {
                var map = obj as Dictionary<String, Object>;
                foreach (var e in map) {
                    if (e.Value is Dictionary<String, Object> || (e.Value is IEnumerable&&!(e.Value is String))) this[e.Key] = Json.Encode(e.Value);
                    else this[e.Key] = ValueString(e.Value);
                }
            } else if (obj is IEnumerable) {
                var en = obj as IEnumerable;
                int i = 0;
                foreach (var e in en) {
                    if (e is Dictionary<String, Object> || (e is IEnumerable && !(e is String))) this[i.ToString()] = Json.Encode(e);
                    else this[i.ToString()] = ValueString(e);
                    ++i;
                }
            } else {
                return obj.ToString();
            }
            return null;
        }

        private String ValueString(object obj) {            
            var s = obj.ToString();
            return s;
        }
    }
}
