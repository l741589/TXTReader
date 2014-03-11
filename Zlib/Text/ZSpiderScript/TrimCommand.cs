using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    class TrimNode : ZSSCommand{

        public override string Command { get { return "trim"; } }
        public TrimNode(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
        }

        public override string DoExecute(string input) {
            return input.Trim();
        }
    }
}
