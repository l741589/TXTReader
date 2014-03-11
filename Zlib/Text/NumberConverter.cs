using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zlib.Text {
    public static class NumberConverter {
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
            "⒛⒇廿",
            "卅",
            "卌"
        };
        public static readonly String[] NumUnit = 
        {
            "十拾⒑㈩⑩⑽Ⅹ",
            "百佰陌",
            "千仟阡",
            "万萬",
            "亿億",
        };

        public static String AllNums { get { return String.Join("", Num) + String.Join("", NumUnit) + String.Join("", NumEx); } }
        //*/
        #endregion

       
    }
}
