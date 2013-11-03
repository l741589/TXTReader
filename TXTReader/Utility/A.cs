using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TXTReader.Utility {

    static class A {
        public static void CopyText(out String[] text,ICollection<String> src, int piecelen = 4096) {
            if (piecelen == 0) {
                text = src.ToArray();
                return;
            }
            List<String> ss = new List<String>();
            foreach (String s in src) {
                int i = 0;
                for (; i < s.Length - piecelen; i += piecelen) {
                    ss.Add(s.Substring(i, piecelen));
                }
                ss.Add(s.Substring(i));
            }
            text = ss.ToArray();
        }

        public static String CheckDir(String path) {
            if (Directory.Exists(path)) return path;
            String[] paths = path.Split(new char[] { '\\', '/' });
            String dir="";
            foreach (String s in paths)
            {
                dir+=s;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                dir += "/";
            }
            return path;
        }

        public static bool FileExists(String uri){
            if (uri.StartsWith(G.HTTP_HEAD)) return false;
            if (uri.StartsWith(G.FILE_HEAD)) return File.Exists(uri.Substring(G.FILE_HEAD.Length));
            if (uri.StartsWith(G.PACK_HEAD) || uri.StartsWith(G.TXTREADER_HEAD)) return true;
            return File.Exists(uri);
        }

        public static int Max(params int[] args) {
            int x = -0x7fffffff;
            foreach (var y in args) x = Math.Max(x, y);
            return x;
        }

        //第三个参数用作效率控制，当复杂度过高时，采用退化算法。
        public static double FuzzyMatch(String a, String b, int countLimit = 0x7fffffff) {
            if (a == null || a.Length == 0 || b == null || b.Length == 0) return 0;
            if (a.Length * b.Length >= countLimit) {
                if (a.Contains(b)) return Math.Min(1, b.Length / a.Length);
                else return 0;
            }
            int[,] f = new int[a.Length + 1, b.Length + 1];
            for (int i = 1; i <= a.Length; ++i) {
                for (int j = 1; j <= b.Length; ++j) {
                    if (a[i-1] == b[j-1]) f[i, j] = Max(f[i - 1, j - 1] + 1, f[i - 1, j], f[i, j - 1]);
                    else f[i, j] = Max(f[i - 1, j - 1], f[i - 1, j], f[i, j - 1]);
                }
            }
            return f[a.Length, b.Length];
        }

        public static void Resort(ListBox lb) {
            SortDescription[] sds=lb.Items.SortDescriptions.ToArray();
            lb.Items.SortDescriptions.Clear();
            foreach (SortDescription sd in sds) lb.Items.SortDescriptions.Add(sd);
        }
    }
}
