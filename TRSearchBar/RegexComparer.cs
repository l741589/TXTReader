using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TRSearchBar {
    class RegexComparer :ISearchBarComparer{
        public string Name { get { return "正则表达式匹配"; } }

        public object ToolTip {
            get { return "使用正则表达式时，全词匹配设置项无效"; }
        }

        public bool Cmp(string input, string pattern, CmpOption op) {
            if (op.CaseSensitive) return Regex.IsMatch(input, pattern);
            else return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }
    }
}
