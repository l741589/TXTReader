using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zlib.Utility;

namespace Zlib.Text.ZSpiderScript {
    class ReplaceCommand : ZSSCommand{

        public override string Command { get { return "replace"; } }

        public String Pattern { get { return this[0]; } }
        public Regex Regex { get; private set; }
        public String Replacement { get; private set; }
        public ReplaceCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
            Replacement = this[1] == null ? "" : this[1];
            if (ContainsSwich("-e")) {
                Replacement = Replacement.Unescape();
            }
            if (ContainsSwich("-r")) {
                Regex = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            } else {
                Regex = null;
            }
        }



        public override string DoExecute(string input) {
            if (Regex == null) {
                return input.Replace(Pattern, GenVar(Replacement));
            } else {
                return Regex.Replace(input, GenVar(Replacement));
            }
        }
    }
}
