using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Zlib.Utility;

namespace Zlib.Text.ZMatchExpression {
    public class ZME {

        public String Source { get; private set; }
        public Regex Regex { get; private set; }
        public IEnumerable<ZME> Children { get; private set; }

        public void Init(String input) {
            if (input == null) return;
            try {
                Source = input;
                Regex = new Regex("^" + ToRegex(input) + "$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
                Children = null;
            } catch {
                Children = null;
                Regex = null;
                Source = null;
            }
        }

        public void Init(IEnumerable<ZME> children) {
            Source = "[" + String.Join(",", from s in children select s.Source) + "]";
            Regex = null;
            Children = children;
        }

        private static String ToRegex(String s) {
            StringBuilder sb = new StringBuilder();
            bool esc = false;
            bool quote = false;
            
            foreach (var c in s) {
                if (esc) {
                    sb.Append(Regex.Escape(c.ToString()));
                    esc = false;
                } else {
                    switch (c) {
                        case '\\': esc = true; break;
                        case '"':
                            if (quote) sb.Append(")");
                            else sb.Append("(?<C>");
                            quote = !quote;
                            break;
                        case ' ':
                        case '\t': sb.Append(@"(?<S>\s*)"); break;
                        case '#': sb.Append(@"(?<N>" + NumberUtil.AllNumsRegex + ")"); break;
                        case '.': sb.Append(@"."); break;
                        case '*': sb.Append(@".*?"); break;
                        case '+': sb.Append(@".+?"); break;
                        case '?': sb.Append(@".??"); break;
                        case '[': if (quote) sb.Append(@"(?<I_"); else sb.Append("(?<i_"); break;
                        case ']': sb.Append(@">)"); break;
                        case '{': sb.Append(@"(?<D>"); break;
                        case '}': sb.Append(@")"); break;
                        case '<': sb.Append(@"(?:^|(?<D>\s+)).*"); break;
                        case '>': sb.Append(@".*(?:(?=\s)|$)"); break;
                        case '=': sb.Append(@"(?:^|(?<D>\s+)).*(?:(?=\s)|$)"); break;
                        case '(': sb.Append(@"(?:"); break;
                        case ')': sb.Append(@")"); break;
                        case '|': sb.Append(@"|"); break;
                        default: sb.Append(Regex.Escape(c.ToString())); break;
                    }
                }
            }
            return sb.ToString();
        }

        public static ZME Compile(String input) {
            ZME t = new ZME();
            t.Init(input);
            return t.Source == null ? null : t;
        }

        public static ZME Compile(IEnumerable<String> input) {
            ZME t = new ZME();
            Queue<ZME> cs = new Queue<ZME>();
            foreach (String s in input) {
                if (s.IsNullOrWhiteSpace()) continue;
                var r = Compile(s);
                if (r == null) continue;
                cs.Enqueue(r);
            }
            t.Init(cs);
            return t.Source == null ? null : t;
        }

        public static ZME Compile(IEnumerable input) {
            ZME t = new ZME();
            if (input is String) t.Init(input as String);
            Queue<ZME> cs = new Queue<ZME>();
            foreach (var o in input) {
                if (o == null) continue;
                if (o is String) {
                    if (String.IsNullOrWhiteSpace(o as String)) continue;
                    cs.Enqueue(Compile(o as String));
                } else if (o is IEnumerable) cs.Enqueue(Compile(o as IEnumerable));
                else cs.Enqueue(Compile(o.ToString()));
            }
            t.Init(cs);
            return t.Source == null ? null : t;
        }

        public ZMEMatch Match(String input) {
            return new ZMEMatcher().Match(this, input, 0);
        }

        public bool IsMatch(String input) {
            return new ZMEMatcher().IsMatch(this, input);
        }

    }
}
