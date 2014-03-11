using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Zlib.Utility;

namespace Zlib.Text.ZSpiderScript {
    class JsDecodeCommand :ZSSCommand{
        public override string Command {
            get { return "jsdecode"; }
        }

        public JsDecodeCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
        }

        public override string DoExecute(string input) {
            var s = this[0] == null ? input : GenVar(this[0]);
            return s.Unescape();
        }
    }
}
