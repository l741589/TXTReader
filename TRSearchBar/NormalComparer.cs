using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TRSearchBar {
    class NormalComparer : ISearchBarComparer{

        public string Name { get { return "普通匹配"; } }

        public object ToolTip {
            get { return null; }
        }

        public bool Cmp(string input, string pattern, CmpOption op) {
            if (op.CaseSensitive) {
                input = input.ToLower();
                pattern = pattern.ToLower();
            }
            if (op.WholeWord) {
                pattern = "\\b" + Regex.Escape(pattern) + "\\b";
                return Regex.IsMatch(input, pattern);
            } else {
                return input.Contains(pattern);
            }
        }
    }
}
