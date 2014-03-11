using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace Zlib.Text.ZSpiderScript {
    class CaptureCommand : ZSSCommand {
        private Regex R;
        private Regex R2;
        public override string Command { get { return "capture"; } }
        public int? Index;
        public String Exp { get { return this[0]; } }
        public String Exp2;
        public String SimiValue;        

        public CaptureCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
            if (Exp != null) R = new Regex(Exp, RegexOptions.Compiled);
            else throw new ZSSParseException("Need Expression");
            var sw = GetSwich("-i");
            if (sw != null) Index = int.Parse(sw.Arguments[0]);
            sw = GetSwich("-s");
            if (sw != null) {
                Exp2 = sw.Arguments[0];
                R2 = new Regex(Exp2, RegexOptions.Compiled);
                SimiValue = sw.Arguments[1];
            }
        }

        public override string DoExecute(string input) {
            if (ContainsSwich("-j")) {
                var xml = Json.ToXml(input);
                if (ContainsSwich("-i")) return Json.FromXml(XPathCapI(xml));
                if (ContainsSwich("-s")) return Json.FromXml(XPathCapS(xml));
                return Json.FromXml(XPathCap(xml));
            }
            if (ContainsSwich("-xp")) {
                if (ContainsSwich("-i")) return XPathCapI(input);
                if (ContainsSwich("-s")) return XPathCapS(input);
                return XPathCap(input);
            }
            if (ContainsSwich("-i")) return RegexCapI(input);
            if (ContainsSwich("-s")) return RegexCapS(input);
            return RegexCap(input);
        }

        private String JQueryCap(String input) {
            return null;
        }

        Regex R_ATTR = new Regex(@"(?<=@)(?:\w+|\*)$", RegexOptions.Compiled);

        private String NodeToString(HtmlNode n,String exp) {
            if (n.Attributes != null) {
                var m = R_ATTR.Match(exp);
                if (m.Success) {
                    if (Exp == "*") return n.Attributes[0].Value;
                    if (n.Attributes[m.Value] != null) return n.Attributes[m.Value].Value;
                }
            }

            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xs = new XmlWriterSettings();
            xs.OmitXmlDeclaration = true;
            xs.ConformanceLevel = ConformanceLevel.Auto;
            using (var xw = XmlWriter.Create(sb, xs)) n.WriteTo(xw);
            return sb.ToString();
        }

        private String XPathCap(String input) {            
            var x = new HtmlDocument();
            x.LoadHtml(input);
            if (Exp == null) Throw("Need Expression");
            var n = x.DocumentNode.SelectSingleNode(Exp);
            if (n != null) {
                return NodeToString(n, Exp);
            } else {
                Throw("No Nodes Captured");
                return null;
            }            
        }

        private String XPathCapS(String input) {
            var x = new HtmlDocument();
            x.LoadHtml(input);
            if (Exp == null) Throw("Need Expression");
            var ns = x.DocumentNode.SelectNodes(Exp);
            int min = 0x7fffffff;
            String mins = null;
            if (ns != null && ns.Count > 0) {
                foreach (var n in ns) {
                    String s = NodeToString(n, Exp);
                    String ss=null;
                    if (Exp2 != null) {
                        var xx = new HtmlDocument();
                        xx.LoadHtml(s);
                        var nn = xx.DocumentNode.SelectSingleNode(Exp2);
                        if (nn == null) Throw("No Nodes Captured");
                        ss = NodeToString(nn, Exp2);
                    } else {
                        ss = s;
                    }
                    var dis=Algorithm.StringCompare.LevenshteinDistance(ss, GenVar(SimiValue));
                    if (dis < min) {
                        min = dis;
                        mins = s;
                    }
                }
                return mins;
            } else {
                Throw("No Nodes Captured");
                return null;
            }        
        }

        private String XPathCapI(String input) {
            var x = new HtmlDocument();
            x.LoadHtml(input);
            if (Exp == null) Throw("Need Expression");
            var ns = x.DocumentNode.SelectNodes(Exp);
            if (ns!=null&&ns.Count > 0) {
                if (Index.Value < 0) Index = ns.Count + Index.Value;
                if (Index.Value >= ns.Count) Index = ns.Count - 1;
                if (Index.Value < 0) Index = 0;
                var n = ns[Index.Value];
                return NodeToString(n, Exp);
            } else {
                Throw("No Nodes Captured");
                return null;
            }
        }

        private String RegexCap(String input) {
            if (R == null) Throw("Need Expression");
            var m = R.Match(input);
            if (!m.Success) return null;
            foreach (String s in R.GetGroupNames()) {
                var g = m.Groups[s];
                if (g.Success) this[s] = g.Value;
            }
            return m.Value;
        }

        private String RegexCapI(String input) {
            if (R == null) Throw("Need Expression");
            var ms = R.Matches(input);
            if (ms.Count==0) return null;
            if (Index.Value < 0) Index = ms.Count + Index.Value;
            if (Index.Value >= ms.Count) Index = ms.Count - 1;
            if (Index.Value < 0) Index = 0;
            var m = ms[Index.Value];
            foreach (String s in R.GetGroupNames()) {
                var g = m.Groups[s];
                if (g.Success) this[s] = g.Value;
            }
            return m.Value;
        }

        private String RegexCapS(String input) {
            if (R == null) Throw("Need Expression");
            var ms = R.Matches(input);
            if (ms.Count == 0) return null;
            int min = 0x7fffffff;
            Match minm = null;
            foreach (Match m in ms) {
                Match mm = null;
                if (Exp2 != null) {
                    mm = R2.Match(m.Value);
                    if (!mm.Success) return null;
                } else {
                    mm = m;
                }
                int dis = Algorithm.StringCompare.LevenshteinDistance(mm.Value, GenVar(SimiValue));
                if (dis < min) {
                    min = dis;
                    minm = m;
                }
            }
            foreach (String s in R.GetGroupNames()) {
                var g = minm.Groups[s];
                if (g.Success) this[s] = g.Value;
            }
            return minm.Value;
        }

        private void Throw(String s) {
            throw new ZSSRuntimeException(s);
        }
    }
}