using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRSearchBar;

namespace TRBook.Rules {
    class TrmexComparer : ISearchBarComparer {
        public TrmexComparer() {
            SearchBarComparerCollection.Instance.Add(this);
        }

        public string Name {
            get { return "章节划分表达式匹配"; }
        }

        public object ToolTip {
            get { return "使用划分章节所使用的表达式来查找\n该选项下大小写敏感以及全字匹配设置项无效\n只能匹配整行"; }
        }

        public bool Cmp(string input, string pattern, CmpOption op) {
            var t=Trmex.Compile(pattern);
            return t.IsMatch(input);
        }
    }
}
