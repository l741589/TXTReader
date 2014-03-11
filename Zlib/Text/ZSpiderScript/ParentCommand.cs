using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    public abstract class ParentCommand : ZSSCommand, IParentCommand {

        protected List<ZSSCommand> Children { get; set; }
        protected Dictionary<String, String> Variables { get; set; }

        public ParentCommand(ParentCommand parent, CommandDesc cd) : base(parent, cd){
            Variables = new Dictionary<String, String>();
        }

        public override String this[String varName] {
            get {
                if (varName == "$^") if (Parent != null) return Parent["$$"]; else return null;
                int x; if (int.TryParse(varName, out x)) varName = x.ToString();
                if (Variables.ContainsKey(varName)) return Variables[varName];
                if (Parent != null) return Parent[varName];
                return null;
            }
            set {
                int x; if (int.TryParse(varName, out x)) varName = x.ToString();
                if (varName == "$^") {
                    if (Parent!=null)
                        Parent["$$"] = value;
                } else {
                    for (var n = this; n != null; n = n.Parent) {
                        if (n.Variables.ContainsKey(varName)) {
                            n.Variables[varName] = GenVar(value);
                            return;
                        }
                    }
                    Variables[varName] = GenVar(value);
                }
            }
        }

        public override void Execute(string input) {
            try {
                Variables.Add("$$", GenVar(input));
                /*
                base.Execute(input);/*/
                var sw = GetSwich("<");
                if (sw != null && sw.Arguments.Count == 1) {
                    input = GenVar(sw.Arguments[0]);
                    if (Context.Input != null) input = Context.Input(this, new PipeInputEventArgs { Spider = Root, DefaultInput = input, Source = sw.Arguments[0] });
                }  //*/              
                var s = DoExecute(input);                
                //if (Parent != null && Parent is ZSSRoot) Parent["$$"] = this["$$"];
                if (this is CaseCommand) (this as CaseCommand).Result = this["$$"];
            } catch (ZSSRuntimeException) {
                throw;
            } catch (Exception e) {
                throw new ZSSRuntimeException(String.Format("Exception Occurs When Executing Command: '{0}' \r\n{1}", this.ToString(), e.Message), e);
            } finally {
                Variables.Clear();
            }
        }

        public override string DoExecute(string input) {
            ExecuteChilren(input);
            return null;
        }

        protected void ExecuteChilren(String input) {
            Variables["$$"] = input;
            if (input != null) {                
                var sw = GetSwich(">");
                bool outputed = false;
                if (sw != null && sw.Arguments.Count == 1) {
                    if (Context.Output != null)
                        outputed = Context.Output(this, new PipeOutputEventArgs { Spider = Root, Data = input, Target = sw.Arguments[0] });
                    if (!outputed) {
                        String name = CheckTarget(sw.Arguments[0]);
                        if (name != null)
                            this[name] = input;
                    }
                }
            }
            foreach (ZSSCommand n in Children) n.Execute(Variables["$$"]);
        }

        public void AddChild(ZSSCommand node) {
            if (Children==null) Children = new List<ZSSCommand>();
            node.Parent = this;
            Children.Add(node);
        }

        public override object Clone(ParentCommand parent) {
            var n = base.Clone(parent) as ParentCommand;
            foreach (var e in Children) e.Clone(n);
            n.Variables.Clear();
            return n;
        }



        public ParentCommand GetReal() {
            return this;
        }
    }

    internal class ParentCommandProxy<T> : ParentCommand where T : ZSSCommand {

        private String command = null;
        public T Proxy { get; private set;}

        public ParentCommandProxy(ParentCommand parent, T proxy, CommandDesc cd, String command)
            : base(null, cd) {
            Parent = parent;
            Proxy = proxy;
            this.command = command;
        }

        public override string Command { get { return command; } }

        public static implicit operator T(ParentCommandProxy<T> src) {
            return src.Proxy;
        }

        public override object Clone(ParentCommand parent) {
            var n = new ParentCommandProxy<T>(parent, Proxy, CommandDesc, command);
            foreach (var e in Children) e.Clone(n);
            n.Variables.Clear();
            return n;
        }
    }
}