using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace TXTReader.Data {
    class RootChapter : Chapter,ContentAdapter {
        public RootChapter() {

        }

        private Chapter Insert(List<String> subtitles, int level,Chapter node) {
            Chapter ret = null;
            if (level < subtitles.Count) {
                ret=Insert(subtitles, level + 1, (Chapter)node[subtitles[level]]);
            } else {
                ret=node;
            }
            return ret;
        }

        public bool Match(Trmex trmex,ICollection<String> texts){
            Chapter node = this;
            foreach (var s in texts) {
                var r = trmex.Match(s);
                if (r != null) {
                    if (trmex.LCs == null) {
                        node = Insert(r.SubTitle, 0, this);
                    } else {
                        var n = node;
                        while (n.Children != null) n = n.Children.Last.Value as Chapter;
                        while (n.Level < r.Level - 1) n = n["未命名章节"];
                        while (n.Level > r.Level - 1 && n != null) n = n.Parent as Chapter;
                        node = n[r.Title];
                    }
                } else {
                    node.AppendText(s);
                }
            }
            return true;
        }

        public void Load(String file,Trmex trmex) {
            var ss=File.ReadAllLines(file,Encoding.Default);
            Title = Path.GetFileName(file);
            Match(trmex, ss);
        }
    }
}
