using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    class SubStringCommand :ZSSCommand{

        public override string Command { get { return "substring"; } }

        public int From { get; private set; }
        public int? To { get; private set; }

        public SubStringCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
                try {
                    if (this[0] == null) From = 0;
                    else From = int.Parse(this[0]);
                    if (this[1] == null) To = null;
                    else To = int.Parse(this[1]);
                } catch (FormatException) {
                    throw new ZSSParseException("Integer format unacceptable.");
                } catch (OverflowException) {
                    throw new ZSSParseException("Integer is too large or too small");
                }
        }

        public override String DoExecute(String input) {
            
            if (From >= input.Length) return "";
            int from = From;
            int? to = To;
            if (from < 0) from = Math.Max(0, input.Length + from);
            if (to != null && to.Value < 0) to = Math.Max(0, input.Length + to.Value);
            if (to == null) return input.Substring(from);
            else if (to.Value <= from) return "";
            else return input.Substring(from, to.Value - from);
        }
    }
}
