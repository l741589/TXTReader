using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zlib.Text.ZSpiderScript {
    class UrlEncodeNode : ZSSCommand {

        public Encoding Encoding;

        public override string Command {
            get { return "urlencode"; }
        }

        public UrlEncodeNode(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
            var sw = GetSwich("-e");
            if (sw == null) Encoding = Root.Encoding; else Encoding = Encoding.GetEncoding(sw.Arguments[0]);
        }

        public override string DoExecute(string input) {
            return HttpUtility.UrlEncode(this[0] == null ? input : GenVar(this[0]), Encoding);
        }
    }
}
