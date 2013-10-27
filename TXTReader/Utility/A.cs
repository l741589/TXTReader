using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
