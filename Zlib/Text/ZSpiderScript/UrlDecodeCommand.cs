using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zlib.Text.ZSpiderScript {
    class UrlDecodeNode : ZSSCommand {
        public Encoding Encoding;

        public override string Command {
            get { return "urldecode"; }
        }

        public UrlDecodeNode(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
            var sw = GetSwich("-e");
            if (sw == null) Encoding = Root.Encoding; else Encoding = Encoding.GetEncoding(sw.Arguments[0]);
        }

        public override string DoExecute(string input) {
            return HttpUtility.UrlDecode(this[0] == null ? input : GenVar(this[0]), Encoding);
        }
    }
}
