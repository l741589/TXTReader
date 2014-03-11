using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    class CallCommand : ParentCommand{

        public override string Command { get { return "call"; } }

        public String CaseName { get { return GenVar(this[0]); } }

        public CallCommand(ParentCommand parent, CommandDesc cd) : base(parent,cd){

        }

        public override String DoExecute(String input) {
            if (Children == null) {
                var n = Root.GetCase(CaseName == null ? input : CaseName);
                if (n == null) throw new ZSSRuntimeException("Invalid Case Name");
                n.Clone(this);
            }
            return base.DoExecute(input);
        }
    }
}