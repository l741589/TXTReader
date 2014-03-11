using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Text.ZSpiderScript;

namespace Zlib.Text.ZSpiderScript {
    class IfCommand : ZSSCommand, IParentCommand{

        public override string Command {
            get { return "if"; }
        }
        //作为其中一个ParentCommand的Proxy
        public ParentCommand Then { get; private set; }
        public ParentCommand Else { get; set; }
        private JudgeUnit J;

        public IfCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
            Then = new ParentCommandProxy<IfCommand>(Parent,this, CommandDesc, Command);
            Else = null;
            J = new JudgeUnit(Arguments, GenVar);
        }

        public void CreateElse(){
            Else = new ParentCommandProxy<IfCommand>(Parent, this, CommandDesc, Command);

        }

        public override string DoExecute(string input) {
            if (J.Execute(input, ContainsSwich("-not"))) return Then.DoExecute(input);
            else if (Else != null) return Else.DoExecute(input);
            else return null;
        }

        public void AddChild(ZSSCommand node) {
            GetReal().AddChild(node);
        }

        public ParentCommand GetReal() {
            if (Else == null) return Then; else return Else;
        }

        public override object Clone(ParentCommand parent) {
            var n=base.Clone(parent) as IfCommand;
            n.Then = Then != null ? Then.Clone(parent) as ParentCommand: null;
            n.Else = Else != null ? Else.Clone(parent) as ParentCommand : null;
            return n;
        }
    }
}
