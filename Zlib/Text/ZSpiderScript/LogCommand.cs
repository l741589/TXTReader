using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    class LogCommand : ZSSCommand {

        public override string Command {
            get { return "log"; }
        }
        
        public LogCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {

        }

        public LogCommand(String log)
            : base(null, null) {
                this[0] = log;
        }

        public override string DoExecute(string input) {
            if (Root != null && Root.Context != null && Root.Context.Logger != null)
                if (this[0] == null) Root.Context.Logger(this, new LogEventArgs { Text = input });
                else Root.Context.Logger(this, new LogEventArgs { Text = GenVar(this[0]) });
            return null;
        }
    }
}
