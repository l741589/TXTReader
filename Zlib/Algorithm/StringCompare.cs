using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Utility;

namespace Zlib.Algorithm {
    public static class StringCompare {

        public static int LongestCommonSubstringLength(string s1, string s2) {
            if (s1 == null) return 0;
            if (s1 == s2) return s1.Length;
            else if (String.IsNullOrEmpty(s1) || String.IsNullOrEmpty(s2)) return 0;
            var d = new int[s1.Length, s2.Length];
            var length = 0;
            for (int i = 0; i < s1.Length; i++) {
                for (int j = 0; j < s2.Length; j++) {
                    // 左上角值
                    var n = i - 1 >= 0 && j - 1 >= 0 ? d[i - 1, j - 1] : 0;
                    // 当前节点值 = "1 + 左上角值" : "0"
                    d[i, j] = s1[i] == s2[j] ? 1 + n : 0;
                    // 如果是最大值，则记录该值和行号
                    if (d[i, j] > length) {
                        length = d[i, j];
                    }
                }
            }
            return length;
        }

        public static string LongestCommonSubstring(string s1, string s2) {
            if (s1 == s2) return s1;
            else if (String.IsNullOrEmpty(s1) || String.IsNullOrEmpty(s2)) return null;
            var d = new int[s1.Length, s2.Length];
            var index = 0;
            var length = 0;
            for (int i = 0; i < s1.Length; i++) {
                for (int j = 0; j < s2.Length; j++) {
                    // 左上角值
                    var n = i - 1 >= 0 && j - 1 >= 0 ? d[i - 1, j - 1] : 0;
                    // 当前节点值 = "1 + 左上角值" : "0"
                    d[i, j] = s1[i] == s2[j] ? 1 + n : 0;
                    // 如果是最大值，则记录该值和行号
                    if (d[i, j] > length) {
                        length = d[i, j];
                        index = i;
                    }
                }
            }
            return s1.Substring(index - length + 1, length);
        }

        public static int LongestCommonSubsequenceLength<E>(E[] s1, E[] s2) {
            int[,] num = new int[s1.Length + 1, s2.Length + 1];
            for (int i = 1; i < s1.Length + 1; i++) {
                for (int j = 1; j < s2.Length + 1; j++) {
                    if (s1[i - 1].Equals(s2[j - 1])) {
                        num[i, j] = 1 + num[i - 1, j - 1];
                    } else {
                        num[i, j] = Math.Max(num[i - 1, j], num[i, j - 1]);
                    }
                }
            }
            return num[s1.Length, s2.Length];
        }
 
        public static List<E> LongestCommonSubsequence<E>(E[] s1, E[] s2) {
            int[,] num = new int[s1.Length + 1, s2.Length + 1];
            for (int i = 1; i < s1.Length + 1; i++) {
                for (int j = 1; j < s2.Length + 1; j++) {
                    if (s1[i - 1].Equals(s2[j - 1])) {
                        num[i, j] = 1 + num[i - 1, j - 1];
                    } else {
                        num[i, j] = Math.Max(num[i - 1, j], num[i, j - 1]);
                    }
                }
            }
            Debug.WriteLine("lenght of LCS= " + num[s1.Length, s2.Length]);
            int s1position = s1.Length, s2position = s2.Length;
            LinkedList<E> result = new LinkedList<E>();
            while (s1position > 0 && s2position > 0) {
                if (s1[s1position - 1].Equals(s2[s2position - 1])) {
                    result.AddLast(s1[s1position - 1]);
                    s1position--;
                    s2position--;
                } else if (num[s1position, s2position - 1] >= num[s1position - 1, s2position]) {
                    s2position--;
                } else {
                    s1position--;
                }
            }
            return result.Reverse().ToList();
        }

        public static int LevenshteinDistance(string s1, string s2) {
            if (s1 == s2) return 0;
            else if (String.IsNullOrEmpty(s1)) return s2.Length;
            else if (String.IsNullOrEmpty(s2)) return s1.Length;
            var m = s1.Length + 1;
            var n = s2.Length + 1;
            var d = new int[m, n];
            // Step1
            for (var i = 0; i < m; i++) d[i, 0] = i;
            // Step2
            for (var j = 0; j < n; j++) d[0, j] = j;
            // Step3
            for (var i = 1; i < m; i++) {
                for (var j = 1; j < n; j++) {
                    var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                    var deletion = d[i - 1, j] + 1;
                    var insertion = d[i, j - 1] + 1;
                    var substitution = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(deletion, insertion), substitution);
                }
            }
            return d[m - 1, n - 1];
        }
    }

}
