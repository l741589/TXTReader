using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Zlib.Text.ZSpiderScript {
    class JsEncodeCommand :ZSSCommand{
        public override string Command {
            get { return "jsencode"; }
        }

        public JsEncodeCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
        }

        public override string DoExecute(string input) {
            return HttpUtility.JavaScriptStringEncode(this[0] == null ? input : GenVar(this[0]));            
        }
    }
}
