using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zlib.Text {
    public static class NumberUtil {

        public static String[] Num { get; private set; }
        public static String[] NumEx { get; private set; }
        public static String[] NumUnit { get; private set; }
        public static String AllNums { get; private set; }
        public static String AllNumsRegex { get; private set; }

        static NumberUtil() {
            Num = new String[]{
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
            NumEx = new String[]{
                "⒒⑾Ⅺ",
                "⒓⑿Ⅻ",
                "⒔⒀",
                "⒕⒁",
                "⒖⒂",
                "⒗⒃",
                "⒘⒄",
                "⒙⒅",
                "⒚⒆",
                "⒛⒇廿",
                "卅",
                "卌"
            };
            NumUnit = new String[]  {
                "十拾⒑㈩⑩⑽Ⅹ",
                "百佰陌",
                "千仟阡",
                "万萬",
                "亿億",
            };
            AllNums = String.Join("", Num) + String.Join("", NumUnit) + String.Join("", NumEx);
            AllNumsRegex = "[" + AllNums + "]+";
        }

        private static readonly String[] cNumUnit = { "+", "%", "K", "W", "E" };
        public static int? ToNumber(String input) {
            if (input == null || input == "") return null;
            String s = input;
            for (int i = 0; i < 10; ++i)
                foreach (var c in Num[i])
                    s = s.Replace(c.ToString(), i.ToString());
            for (int i = 0; i < 5; ++i)
                foreach (var c in NumUnit[i])
                    s = s.Replace(c.ToString(), cNumUnit[i]);
            for (int i = 1; i < 10; ++i)
                foreach (var c in NumEx[i - 1])
                    s = s.Replace(c.ToString(), cNumUnit[0] + i);
            for (int i = 2; i <= 4; ++i)
                foreach (var c in NumEx[i + 7])
                    s = s.Replace(c.ToString(), i + cNumUnit[0]);

            if (String.Join("", cNumUnit).Contains(s[0])) {
                s = 1 + s;
            }
            if (!String.Join("", cNumUnit).Contains(s[s.Length - 1])) {
                int i = s.LastIndexOfAny(new char[] { 'W', 'E' });
                if (i != -1) {
                    if (s.Length > i + 1 && s[i + 1] != '0') {
                        switch (s[i]) {
                            case 'W': s += 'K'; break;
                            case 'E': s += "KW"; break;
                        }
                    }
                }
            }

            int N = 0;
            int W = 0;
            int E = 0;
            String[] ss = s.Split('E');
            if (ss.Length > 1) { E = NumberLevel(ss[0]); s = ss[1]; }
            ss = s.Split(cNumUnit[3][0]);
            if (ss.Length > 1) { W = NumberLevel(ss[0]); s = ss[1]; }
            N = NumberLevel(s);
            return E * 100000000 + W * 10000 + N;
        }

        private static int NumberLevel(String input) {
            int sum = 0;
            int x = 0;
            bool zeroed = false;
            foreach (char c in input) {
                switch (c) {
                    case '+': sum += x * 10; x = 0; break;
                    case '%': sum += x * 100; x = 0; break;
                    case 'K': sum += x * 1000; x = 0; break;
                    case '0': x *= 10; x += 0; zeroed = true; break;
                    case '1': x *= 10; x += 1; break;
                    case '2': x *= 10; x += 2; break;
                    case '3': x *= 10; x += 3; break;
                    case '4': x *= 10; x += 4; break;
                    case '5': x *= 10; x += 5; break;
                    case '6': x *= 10; x += 6; break;
                    case '7': x *= 10; x += 7; break;
                    case '8': x *= 10; x += 8; break;
                    case '9': x *= 10; x += 9; break;
                }
            }
            if (x != 0) {
                if (zeroed) sum += x;
                else {
                    if (sum == 0) sum += x;
                    else if (sum % 10000 == 0) { while (x * 10 < 10000) x *= 10; sum += x; } else
                        if (sum % 1000 == 0) { while (x * 10 < 1000) x *= 10; sum += x; } else
                            if (sum % 100 == 0) { while (x * 10 < 100) x *= 10; sum += x; } else
                                if (sum % 10 == 0) { while (x * 10 < 10) x *= 10; sum += x; }
                }
            }
            return sum;
        }
    }
}
