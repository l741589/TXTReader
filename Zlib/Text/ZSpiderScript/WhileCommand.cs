using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    class WhileCommand : ParentCommand{
        public override string Command { get { return "while"; } }
        private JudgeUnit J;

        public WhileCommand(ParentCommand parent, CommandDesc cd) :base(parent,cd) {
            J = new JudgeUnit(Arguments, GenVar);
        }

        public override void Execute(string input) {
            var s = input;
            try {
                while (J.Execute(input, ContainsSwich("-not"))) {
                    Variables.Add("$$", GenVar(input));
                    s = DoExecute(input);
                    Variables.Clear();
                }
                if (Parent != null&&s!=null) Parent["$$"] = s;
            } catch (ZSSRuntimeException) {
                throw;
            } catch (Exception e) {
                throw new ZSSRuntimeException(String.Format("Exception Occurs When Executing Command: '{0}' \r\n{1}", this.ToString(), e.Message), e);
            } finally {
                Variables.Clear();
            }
        }
    }
}
