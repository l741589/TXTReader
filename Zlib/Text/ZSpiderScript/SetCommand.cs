using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Zlib.Utility;

namespace Zlib.Text.ZSpiderScript {

    class SetCommand : ZSSCommand{

        public override string Command { get { return "set"; } }

        

        /*public const String S_TITLE = "title";
        public const String S_AUTHOR = "author";
        public const String S_ID = "id";
        public const String S_ENTRY = "entry";
        public const String S_COVER = "cover";
        public const String S_CONTENT = "content";
        public const String S_URL = "url";*/

        

        public String Target { get; private set; }
        public String Value { get { return this[1]; } }

        public SetCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
            Target = CheckTarget(this[0]);
        }

        public override string DoExecute(string input) {
            this[Target] = Value != null ? Value : input;
            return this[Target];
        }
    }
}
