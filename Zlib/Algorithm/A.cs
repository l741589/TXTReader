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
using System.Web;
using System.Windows.Controls;

namespace Zlib.Algorithm {

    static public class A {
        public const String HTTP_HEAD = "http://"; 
        public const String FILE_HEAD = "file:///";
        public const String PACK_HEAD = "pack://"; 
       

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

        public static String CheckExt(String path,String ext) {
            if (Path.GetExtension(path) == ext) return path;
            else return path + ext;
        }

        public static bool FileExists(String uri){
            if (uri.StartsWith(HTTP_HEAD)) return false;
            if (uri.StartsWith(FILE_HEAD)) return File.Exists(uri.Substring(FILE_HEAD.Length));
            if (uri.StartsWith(PACK_HEAD)) return true;
            if (uri.StartsWith("/")) return true;
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

        private static readonly Regex R_FILENAME_DECODER = new Regex(@"%[0-9a-fA-F]{2}");

        public static bool IsFilename(String filename) {
            return filename.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
        }

        public static bool IsFullFilename(String filename) {
            return filename.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }

        public static String EncodeFilename(String filename) {
            //return HttpUtility.UrlEncode(filename);
            String s = filename;
            s = s.Replace("%", String.Format("%{0:x2}", (short)'%'));
            foreach (var c in Path.GetInvalidFileNameChars()) {
                s = s.Replace(c.ToString(), String.Format("%{0:x2}", (short)c));
            }            
            return s;
        }

        public static String DecodeFilename(String filename) {
            //return HttpUtility.UrlDecode(filename);
            return R_FILENAME_DECODER.Replace(filename, (n) => {
                char c=(char)int.Parse(n.Value.Substring(1), System.Globalization.NumberStyles.HexNumber);
                return c.ToString();
            });
        }

        public static String MD5(byte[] data) {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var md5data = md5.ComputeHash(data);
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length ; i++) {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }

        public static T WorkWith<T>(T value,Func<T,T> work) {
            if (work == null) return value;
            return work(value);
        }

        public static T WorkWith<T>(Func<T> work) {
            if (work == null) return default(T);
            return work();
        }
    }
}
