using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TXTReader.Data
{
    enum MatchType { NoMatch, List, Tree, Both };
    enum MatchLang { Trmex, Regex };
    class Chapter
    {
        
        private String title = null;
        public Chapter[] Children { get; set; }
        public Chapter this[int i] { get { return Children[i]; } set { Children[i] = value; } }
        public List<String> Text { get; set; }
        public MatchType MatchType { get; set; }

        public String Title
        {
            get
            {
                if (title == null && Text != null && Text.Count > 0) return Text[0];
                return title;
                
            }
            set
            {
                title = value;
            }
        }

        public Chapter(IEnumerable<String> text = null, String pattern = null, MatchType matchType = MatchType.List,MatchLang lang=MatchLang.Regex)
        {
            Text = text.ToList();
            if (Text == null) return;
            if (pattern == null) matchType = MatchType.NoMatch;
            else Match(pattern, matchType, lang);
        }

        public static void Match(String pattern, MatchType matchType, MatchLang lang)
        {
            
        }
    }
}
