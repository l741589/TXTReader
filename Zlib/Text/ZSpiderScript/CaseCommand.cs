using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    class CaseCommand : ParentCommand {

        public String Result { get; set; }

        public override string Command {
            get { return "case"; }
        }

        public CaseCommand(ParentCommand parent, CommandDesc cd)
            : base(null, cd) {
            Parent = parent;
            if (Parent  == null) throw new ZSSParseException("Parent of 'case' must be root");
            if (!(Parent is ZSSRoot)) throw new ZSSParseException("Parent of 'case' must be root");
            if (!(Parent as ZSSRoot).ContainsCase(Arguments[0])) {
                Parent.AddChild(this);
            }
            nodeCase = this[0];
        }

        
    }
}
