using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using TXTReader.Data;

namespace TXTReader.Utility {

    static public class A {
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

        private static readonly String[] filenameReservedWord = new String[]{ "%", "\\", "/", "*", "?", "\"", "<", ">", "|", ":" };
        private static readonly char[] filenameReservedChar = new char[] { '\\', '/', '*', '?', '"', '<', '>', '|', ':' };
        private static readonly char[] fullFilenameReservedChar = new char[] { '*', '?', '"', '<', '>', '|' };
        private static readonly Regex R_FILENAME_DECODER = new Regex(@"%[0-9a-fA-F]{2}");

        public static bool IsFilename(String filename) {
            return filename.IndexOfAny(filenameReservedChar) == -1;
        }

        public static bool IsFullFilename(String filename) {
            return filename.IndexOfAny(fullFilenameReservedChar) == -1;
        }

        public static String EncodeFilename(String filename) {
            String s = filename;
            foreach (var c in filenameReservedWord) {
                s = s.Replace(c, String.Format("%{0:x2}", (short)c[0]));
            }
            return s;
        }

        public static String DecodeFilename(String filename) {
            return R_FILENAME_DECODER.Replace(filename, (n) => {
                char c=(char)int.Parse(n.Value.Substring(1), System.Globalization.NumberStyles.HexNumber);
                return c.ToString();
            });
        }

        public static void ReplaceBook(ref Book book,Book value) {
            if (book == value) return;
            if (book != null) book.Close();
            book = value;
            if (book != null) {
                book.Load();
                G.FloatMessagePanel.UpdateBinding();
            }
            G.MainWindow.toolPanel.pn_bookmark.lb_bookmark.ItemsSource = G.Bookmark;
        }
    }
}
