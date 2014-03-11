using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using HtmlAgilityPack;

namespace Zlib.Text.ZSpiderScript {
    class MultiCaptureCommand : ParentCommand{
        private Regex R;
        public override string Command { get { return "multicapture"; } }

        public String Exp { get { return this[0]; } }

        public MultiCaptureCommand(ParentCommand parent, CommandDesc cd)
            : base(parent, cd) {
            if (Exp != null) R = new Regex(Exp, RegexOptions.Compiled);
        }

        public override string DoExecute(string input) {
            if (ContainsSwich("-xp")) XPathCap(input);
            RegexCap(input);
            return null;
        }

        Regex R_ATTR = new Regex(@"(?<=@)(?:\w+|\*)$", RegexOptions.Compiled);

        private void XPathCap(String input) {
            var x = new HtmlDocument();
            x.LoadHtml(input);
            var ns = x.DocumentNode.SelectNodes(Exp);
            if (ns != null) {
                XmlWriterSettings xs = new XmlWriterSettings();
                xs.OmitXmlDeclaration = true;
                xs.ConformanceLevel = ConformanceLevel.Auto;
                foreach (HtmlNode n in ns) {
                    StringBuilder sb = new StringBuilder();
                    using (var xw = XmlWriter.Create(sb, xs)) n.WriteTo(xw);
                    ExecuteChilren(sb.ToString());
                }
            }
        }

        private void RegexCap(String input) {
            if (R == null) Throw("Need Expression");
            var ms = R.Matches(input);
            foreach (Match m in ms) {
                if (!m.Success) continue;
                foreach (String s in R.GetGroupNames()) {
                    var g = m.Groups[s];
                    if (g.Success) this[s] = g.Value;
                }
                ExecuteChilren(m.Value);
            }
        }

        private void Throw(String s) {
            throw new ZSSRuntimeException(s);
        }
    }
}
