using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Net;

namespace Zlib.Utility {
    public static class StringExtension {
        public static bool IsNullOrEmpty(this String s){
            return String.IsNullOrEmpty(s);
        }

        public static bool IsNullOrWhiteSpace(this String s) {
            return String.IsNullOrWhiteSpace(s);
        }

        public static String Escape(this String s) {
            StringBuilder sb = new StringBuilder();
            foreach (var c in s) {
                switch (c) {
                    case '\a': sb.Append(@"\a"); break;
                    case '\b': sb.Append(@"\b"); break;
                    case '\f': sb.Append(@"\f"); break;
                    case '\n': sb.Append(@"\n"); break;
                    case '\r': sb.Append(@"\r"); break;
                    case '\t': sb.Append(@"\t"); break;
                    case '\v': sb.Append(@"\v"); break;
                    case '\\': sb.Append(@"\\"); break;
                    case '\'': sb.Append(@"\'"); break;
                    case '\"': sb.Append(@"\"""); break;
                    case '\0': sb.Append(@"\0"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        public static String Unescape(this String s) {
            bool esc = false;
            StringBuilder sb = new StringBuilder();
            foreach (var c in s) {
                if (esc) {
                    switch (c) {
                        case 'a': sb.Append('\a'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'v': sb.Append('\v'); break;
                        case '\'': sb.Append('\''); break;
                        case '"': sb.Append('\"'); break;
                        case '0': sb.Append('\0'); break;
                        default: sb.Append(c); break;
                    }
                    esc = false;
                } else {
                    if (c == '\\') esc = true;
                    else sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static UrlArgs eq(this String key, object value) {
            if (key == null || value == null) return null;
            return new UrlArgs(key, value);
        }
    }
}
