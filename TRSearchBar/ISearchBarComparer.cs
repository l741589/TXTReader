using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TRSearchBar {
    //public delegate StringComparer
    public struct CmpOption{
        public bool CaseSensitive;
        public bool WholeWord;
        public bool Circuit;
    }
    public interface ISearchBarComparer {
        String Name { get; }
        object ToolTip { get; }
        bool Cmp(String input, String pattern, CmpOption op);
    }
}
