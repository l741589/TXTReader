using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zlib.Text.ZSpiderScript {
    class HtmlDecodeCommand :ZSSCommand{
        public override string Command {
            get { return "htmldecode"; }
        }

        int Count = 0;
        public HtmlDecodeCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
            if (this[1] != null) {
                int x;
                if (int.TryParse(this[1], out x)) Count = x;
            }
        }

        public override string DoExecute(string input) {
            if (Count > 0) {
                String s = this[0] == null ? input : GenVar(this[0]);
                for (int i = 0; i < Count; ++i) s = HttpUtility.HtmlDecode(s);
                return s;
            }
            return HttpUtility.HtmlDecode(this[0] == null ? input : GenVar(this[0]));
        }
    }
}
