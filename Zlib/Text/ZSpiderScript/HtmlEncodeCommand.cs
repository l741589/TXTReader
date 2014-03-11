using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zlib.Text.ZSpiderScript {
    class HtmlEncodeCommand :ZSSCommand{
        public override string Command {
            get { return "htmlencode"; }
        }

        public HtmlEncodeCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
        }

        public override string DoExecute(string input) {
            return HttpUtility.HtmlEncode(this[0] == null ? input : GenVar(this[0]));            
        }
    }
}
