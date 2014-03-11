using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    public class ZSSRoot : ParentCommand, IZSpider {

        private int reconnect;

        public String Site { get; set; }
        public String Name { get; set; }
        public Encoding Encoding { get; set; }
        public object Tag { get; set; }
        public override string Command { get { return null; } }
        public new ZSS Context { get; set; }
        public int StandardLevel { get; set; }
        public String UserAgent { get; set; }
        public bool IsKeepCookie { get; set; }
        public int Timeout { get; set; }
        public int Reconnect { get { return reconnect; } set { reconnect = value; if (reconnect <= 0) throw new ZSSParseException("重连次数必须为正整数"); } }

        public ZSSRoot() :base(null, null){
            StandardLevel = int.MaxValue;
            Timeout = int.MaxValue;
            Reconnect = 1;
        }

        public bool ContainsCase(String name = null) {
            if (Children == null) return false;
            foreach (ZSSCommand n in Children) {
                if (n is CaseCommand) {
                    if (((CaseCommand)n).Case == name) {
                        return true;
                    }
                }
            }
            return false;
        }

        public ParentCommand GetCase(String name = null) {
            if (Children == null) return null;
            foreach (ZSSCommand n in Children) {
                if (n is CaseCommand) {
                    if (((CaseCommand)n).Case == name) {
                        return (CaseCommand)n.Clone(this);
                    }
                }
            }
            return null;
        }

        public string Execute(String name, String input, object tag = null) {
            String ret = null;
            try {
                //Variables.Add("$$", GenVar(input));
                this.Tag = tag;
                var n = GetCase(name);
                if (n != null) {
                    n.Execute(input);
                    ret = (n as CaseCommand).Result;
                }
                //ret = this["$$"];
            } catch (ZSSRuntimeException) {
                throw;
            } catch (Exception e) {
                throw new ZSSRuntimeException(String.Format("Exception Occurs When Executing Command: '{0}' \r\n{1}", this.ToString(), e.Message), e);
            } finally {
                Variables.Clear();
            }
            return ret;
        }

        public override string DoExecute(string input) {
            Tag = null;
            var n = GetCase();
            if (n != null) n.Execute(input);
            return null;
        }
    }
}
