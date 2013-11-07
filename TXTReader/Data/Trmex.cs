using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TXTReader.Data {
    public class Trmex {
        public class TrmexDesc {
            public String regex;
            public Dictionary<String, String> inserts = new Dictionary<String, String>();
            public int LC;
        }

        #region 数字
        /*
        public static readonly String[] Num = { "一二三四五六七八九零" };
        public static readonly String[] NumUnit = { "十百" };
        /*/
        public static readonly String[] Num =
        {
            "0０〇零洞",
            "1１一壹⒈㈠①⑴Ⅰ壱幺么",
            "2２二贰⒉㈡②⑵Ⅱ弍两",
            "3３三叁⒊㈢③⑶Ⅲ弎叄参參",
            "4４四肆⒋㈣④⑷Ⅳ亖",
            "5５五伍⒌㈤⑤⑸Ⅴ",
            "6６六陆⒍㈥⑥⑹Ⅵ",
            "7７七柒⒎㈦⑦⑺Ⅶ漆質",
            "8８八捌⒏㈧⑧⑻Ⅷ",
            "9９九玖⒐㈨⑨⑼Ⅸ",
        };
        public static readonly String[] NumEx =
        {
            "⒒⑾Ⅺ",
            "⒓⑿Ⅻ",
            "⒔⒀",
            "⒕⒁",
            "⒖⒂",
            "⒗⒃",
            "⒘⒄",
            "⒙⒅",
            "⒚⒆",
            "⒛⒇",
        };
        public static readonly String[] NumUnit = 
        {
            "十拾⒑㈩⑩⑽Ⅹ",
            "百佰陌",
            "千仟阡",
            "万萬",
            "亿億",
            "兆",
        };

        public static String AllNums { get { return String.Join("", Num) + String.Join("", NumUnit) + String.Join("", NumEx); } }
        //*/
        #endregion

        #region 生成匹配用正则表达式
        private static readonly String reservedWord = "\\.+*?[]{}#$% \t\n\r";
        private static readonly Regex R_QUOTE = new Regex(@"(?<!\\)""(?<C>(?:[^""]|\\"")*)(?<!\\)""");
        private static readonly Regex R_DOT = new Regex(@"(?<!\\)\.");
        private static readonly Regex R_STAR = new Regex(@"(?<!\\)\*");
        private static readonly Regex R_PLUS = new Regex(@"(?<!\\)\+");
        private static readonly Regex R_QUES = new Regex(@"(?<!\\)\?");
        private static readonly Regex R_SPACE = new Regex(@"(?<!\\)\s");
        private static readonly Regex R_SHARP = new Regex(@"(?<!\\)#");
        private static readonly Regex R_AT = new Regex(@"(?<!\\)@");
        private static readonly Regex R_EQ = new Regex(@"(?<!\\)=");
        private static readonly Regex R_GT = new Regex(@"(?<!\\)>");
        private static readonly Regex R_LT = new Regex(@"(?<!\\)<");
        private static readonly Regex R_SBRKT = new Regex(@"(?<!\\)\((?<C>(?:[^\)]|\\\))*)(?<!\\)\)");
        private static readonly Regex R_MBRKT = new Regex(@"(?<!\\)\[(?<C>(?:[^\]]|\\\])*)(?<!\\)\]");
        private static readonly Regex R_LBRKT = new Regex(@"(?<!\\)\{(?<C>(?:[^\}]|\\\})*)(?<!\\)\}");

        public static TrmexDesc Precompile(IEnumerable<String> patterns, String prefix = "") {
            List<String> regexs = new List<String>();
            var ret = new TrmexDesc();
            int LC = 0;
            int IC = 0;
            foreach (var pattern in patterns) {
                String regex = pattern;
                if (regex == null || regex == "" || (regex.Length >= 2 && regex[0] == '#' && regex[1] == '#')) {
                    //对于空字符串，或以##开始的注释，忽略
                } else if (regex.First() == '^' && regex.Last() == '$') {
                    //对于以^开头,以$结尾的字符串,直接当做正则表达式处理,其中\#被视为包括汉字的数字
                    //TODO 视为添加的语法，视为添加
                    regex = regex.Substring(1, regex.Length - 2);
                    regex = regex.Replace("\\#", "[" + AllNums + "]+");
                    regexs.Add(regex);
                } else {
                    regex = R_DOT.Replace(regex, "\\\\.");
                    regex = R_STAR.Replace(regex, ".*");
                    regex = R_SPACE.Replace(regex, @"\\s*");
                    regex = R_QUES.Replace(regex, ".?");
                    regex = R_PLUS.Replace(regex, ".+");
                    regex = R_AT.Replace(regex, @"\\b\\w+?\\b");


                    regex = R_LBRKT.Replace(regex, (m) => {
                        String s = m.Groups["C"].Value;
                        return String.Format("(?\\<D\\>{0})", s);
                    });

                    regex = R_QUOTE.Replace(regex, (m) => {
                        String s = m.Groups["C"].Value;
                        return String.Format("(?\\<{0}L{1}\\>{2})", prefix, LC++, s);
                    });
                    regex = R_MBRKT.Replace(regex, (m) => {
                        String s = m.Groups["C"].Value;
                        Debug.Assert(IC == ret.inserts.Count, "Trmex编译错误，描述符大小与编号不合");
                        ret.inserts.Add(String.Format("{0}I{1}", prefix, IC), s);
                        return String.Format("(?\\<{0}I{1}\\>)", prefix, IC++);
                    });

                    regex = R_EQ.Replace(regex, @"\\b[^\\s]+?\\b");
                    regex = R_GT.Replace(regex, @"[^\\s]+?\\b");
                    regex = R_LT.Replace(regex, @"\\b.*");
                    regex = R_SHARP.Replace(regex, "[" + AllNums + "]+");
                    regex = "(?:" + regex + ")";
                    regex = Regex.Unescape(regex);
                    regexs.Add(regex);
                }
            }
            ret.regex = String.Join("|", regexs);
            ret.LC = LC;
            return ret;
        }
        #endregion

        public Dictionary<String, String> inserts = new Dictionary<String, String>();
        public Regex regex;
        public List<int> LCs = null;
        public int LC = 0;

        //List模式
        public Trmex(IEnumerable<String> patterns) {
            TrmexDesc td = Precompile(patterns);
            regex = new Regex("^(?<ALL>" + td.regex + ")$");
            inserts = td.inserts;
            LC = td.LC;
            LCs = null;
        }

        public static Trmex Compile(IEnumerable<String> patterns) {
            return new Trmex(patterns);
        }

        //Tree模式
        public Trmex(IEnumerable<IEnumerable<String>> patterns) { compile(patterns.ToArray()); }

        private void compile(IEnumerable<String>[] patterns) {
            LCs = new List<int>();
            LC = 0;
            List<String> regexs = new List<String>();
            for (int i = 0; i < patterns.Count(); ++i) {
                TrmexDesc td = Precompile(patterns[i], "T" + i);
                inserts = inserts.Union(td.inserts) as Dictionary<String, String>;
                regexs.Add("(?:" + td.regex + ")");
                LCs.Add(td.LC);
            }
            regex = new Regex("^(?<ALL>" + String.Join("|", regexs) + ")$");
        }

        public static Trmex Compile(IEnumerable<IEnumerable<String>> patterns) {
            return new Trmex(patterns);
        }

        public ChapterDesc Match(String src) {
            ChapterDesc cd = new ChapterDesc();
            String ret = src;
            Match m = regex.Match(src);
            if (!m.Success) return null;
            Group g = m.Groups["D"];
            CaptureCollection captureToBeRemove = g.Captures;
            Dictionary<Capture, String> captureToBeInserted = new Dictionary<Capture, String>();
            foreach (Capture e in captureToBeRemove) ret = ret.Remove(e.Index, e.Length);
            foreach (var e in inserts) {
                Group gg = m.Groups[e.Key];
                foreach (Capture ee in gg.Captures) {
                    ret = ret.Insert(ee.Index, e.Value);
                    captureToBeInserted.Add(ee, e.Value);
                }
            }
            cd.Title = ret;
            cd.Level = -1;
            if (LCs == null) {
                for (int i = 0; i < LC; ++i) {
                    foreach (Capture c in m.Groups["L" + i].Captures) {
                        if (c == null || c.Value == null || c.Value == "") continue;
                        String s = c.Value;
                        foreach (Capture e in captureToBeRemove)
                            if (e.Index >= c.Index && e.Index < c.Index + c.Length)
                                s = s.Remove(e.Index - c.Index, e.Length);
                        foreach (KeyValuePair<Capture, String> e in captureToBeInserted)
                            if (e.Key.Index >= c.Index && e.Key.Index < c.Index + c.Length)
                                s = s.Insert(e.Key.Index - c.Index, e.Value);
                        cd.SubTitle.Add(s);
                    }
                }
            } else {
                for (int j = 0; j < LCs.Count; ++j) {
                    int cnt = LCs[j];
                    for (int i = 0; i < cnt; ++i) {
                        foreach (Capture c in m.Groups["T" + j + "L" + i].Captures) {
                            if (c == null || c.Value == null || c.Value == "") continue;
                            String s = c.Value;
                            foreach (Capture e in captureToBeRemove)
                                if (e.Index >= c.Index && e.Index < c.Index + c.Length)
                                    s = s.Remove(e.Index - c.Index, e.Length);
                            foreach (KeyValuePair<Capture, String> e in captureToBeInserted)
                                if (e.Key.Index >= c.Index && e.Key.Index < c.Index + c.Length)
                                    s = s.Insert(e.Key.Index - c.Index, e.Value);
                            cd.SubTitle.Add(s);
                        }
                    }
                    if (cd.SubTitle.Count > 0) {
                        cd.Level = j;
                        break;
                    }
                }
            }
            return cd;
        }
    }
}
