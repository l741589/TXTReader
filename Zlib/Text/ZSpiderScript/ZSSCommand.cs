using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Zlib.Utility;

namespace Zlib.Text.ZSpiderScript {
    public abstract class ZSSCommand : IZSSCommand {

        public static readonly Regex R_VAR = new Regex(@"(?:\$\((?<V>[a-zA-Z_0-9]+)\))|(?<V>\$\$)|(?<V>\$\^)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        public static readonly Regex R_IDENTIFY = new Regex(@"^(?<V>[a-zA-Z_][a-zA-Z0-9_]*)$|^\$\((?<V>[a-zA-Z0-9_]+)\)$|^(?<V>\$\$)$|^(?<V>\$\^)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        public static readonly Regex R_HOSTVAR = new Regex(@"^(?<V>\$[a-zA-Z_][a-zA-Z0-9_]*)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public ParentCommand Parent { get; internal set; }
        public ZSS Context { get { return Root == null ? null : Root.Context; } }

        public abstract String Command { get; }
        public List<String> Arguments { get; private set; }
        public List<ZSSSwitch> Switches { get; private set; }
        protected String nodeCase = null;
        private ZSSRoot root;
        protected CommandDesc CommandDesc { get; set; }

        private Zlib.Net.ZWeb web;

        public ZSSCommand(ParentCommand parent, CommandDesc cd) {
            if (parent!=null) parent.AddChild(this);
            CommandDesc = cd;
            if (cd != null) {
                Arguments = cd.Arguments;
                Switches = cd.Switches;
            }
        }

        protected Zlib.Net.ZWeb W {
            get {
                if (web == null) {
                    if (this is CaseCommand) {
                        web = new Net.ZWeb();
                        web.BaseAddress = Root.Site;
                        web.AllowAutoRedirect = true;
                        web.IsKeepCookie = Root.IsKeepCookie;
                        web.Timeout = Root.Timeout;
                        web.UserAgent = Root.UserAgent;
                    } else web = Parent.W;                    
                }
                return web;
            }
        }

        public ZSSRoot Root {
            get {
                if (root != null) return root;
                if (this.Parent == null && this is ZSSRoot) return root = (ZSSRoot)this;
                else if (this.Parent != null) return root = Parent.Root;
                else return null;
            }
        }

        public String Case {
            get {
                if (nodeCase != null) return nodeCase;
                if (Parent != null) return nodeCase = Parent.Case;
                return null;
            }
        }

        public bool ContainsSwich(String name) {
            var s = (Command + name).ToLower();
            foreach (ZSSSwitch zs in Switches) if (zs.SysName == s) return true;
            return false;
        }

        public ZSSSwitch GetSwich(String name) {
            var s = (Command + name).ToLower();
            foreach (ZSSSwitch zs in Switches) if (zs.SysName == s) return zs;
            return null;
        }

        public List<ZSSSwitch> GetSwiches(String name) {
            List<ZSSSwitch> ret = new List<ZSSSwitch>();
            var s = (Command + name).ToLower();
            foreach (ZSSSwitch zs in Switches) if (zs.SysName == s) ret.Add(zs);
            return ret;
        }

        public String GenVar(String input) {
            if (input.IsNullOrWhiteSpace()) return input;
            if (input == "$null") return null;
            if (Context != null && Context.HostVarHandler != null) {
                var hm = R_HOSTVAR.Match(input);
                if (hm.Success) {
                    return Context.HostVarHandler(this, new HostVarEventArgs { Spider = Root, Name = hm.Groups["V"].Value.Substring(1), Value = null, IsSet = false });
                }
            }
            var s = R_VAR.Replace(input, m => {
                String key = m.Groups["V"].Value;
                if (this[key] != null)
                    return this[key];
                return m.Value;
            });
            return s;
        }

        public virtual String this[String varName] {
            get {
                if (Parent != null) return Parent[varName];
                return null;
            }
            set {
                if (varName.IsNullOrWhiteSpace()) return;
                if (varName[0] == '$' && (varName[1] != '$'&&varName[1]!='^')) {
                    String name = varName.Substring(1);
                    if (Context != null) {
                        if (Context.HostVarHandler != null)
                            Context.HostVarHandler(this, new HostVarEventArgs() { Spider = Root, Name = name, Value = GenVar(value), IsSet = true });
                    }
                } else {
                    Parent[varName] = value;
                }
            }
        }

        public String this[int argIndex] {
            get {
                if (argIndex >= Arguments.Count) return null;
                else return Arguments[argIndex];
            }
            protected set {
                if (Arguments == null) Arguments = new List<string>();
                if (argIndex < Arguments.Count) Arguments[argIndex] = value;
                else {
                    while (Arguments.Count < argIndex) Arguments.Add(null);
                    Arguments.Add(value);
                }
            }
        }

        public int ArgCount() {
            if (Arguments == null) return 0;
            return Arguments.Count;
        }

        public virtual void Execute(String input) {
            try {
                var sw = GetSwich("<");
                if (sw != null && sw.Arguments.Count == 1) {
                    input = GenVar(sw.Arguments[0]);
                    if (Context.Input != null) input = Context.Input(this, new PipeInputEventArgs { Spider = Root, DefaultInput = input, Source = sw.Arguments[0] });
                }
                var s = DoExecute(input);
                if (s != null) {
                    this["$$"] = s;
                    sw = GetSwich(">");
                    bool outputed = false;
                    if (sw != null && sw.Arguments.Count == 1) {
                        if (Context.Output != null)
                            outputed = Context.Output(this, new PipeOutputEventArgs { Spider = Root, Data = s, Target = sw.Arguments[0] });
                        if (!outputed) {
                            String name = CheckTarget(sw.Arguments[0]);
                            if (name != null)
                                this[name] = s;
                        }
                    }
                }
            } catch (Exception e) {
                throw new ZSSRuntimeException(String.Format("Exception Occurs When Executing Command: '{0}' \r\n{1}", this.ToString(), e.Message), e);
            }
        }

        protected String CheckTarget(String name) {
            String Target;
            Match m = R_IDENTIFY.Match(name);
            if (m.Success) {
                Target = m.Groups["V"].Value;
            } else {
                m = R_HOSTVAR.Match(name);
                if (m.Success)
                    Target = m.Groups["V"].Value;
                else Target = null;
            }
            return Target;
        }

        public abstract String DoExecute(String input);

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(Command);

            if (Arguments != null) {
                sb.Append(" ");
                sb.Append(String.Join(" ", Arguments));
            }
            if (Switches != null) {
                sb.Append(" ");
                sb.Append(String.Join(" ", Switches));
            }
            if (this["$$"] != null) {
                sb.Append(" {");
                var s = this["$$"];
                if (s.Length > 256) sb.Append(s.Substring(0, 256));
                else sb.Append(s);
                sb.Append(" }");
            }
            return sb.ToString();
        }

        public virtual object Clone(ParentCommand parent) {
            return GetType().GetConstructor(new Type[] { typeof(ParentCommand), typeof(CommandDesc) }).Invoke(new object[] { parent, CommandDesc });
        }
    }

}
