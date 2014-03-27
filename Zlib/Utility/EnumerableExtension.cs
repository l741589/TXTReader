using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Algorithm;

namespace Zlib.Utility {
    public static class EnumerableExtension {

        public static bool ContainsAny<T>(this IEnumerable<T> t, IEnumerable<T> p) {
            foreach (T e in p) if (t.Contains(e)) return true;
            return false;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> t, IEnumerable p) {
            foreach (T e in p) if (t.Contains(e)) return true;
            return false;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> t, IEnumerable<T> p) {
            foreach (T e in p) if (!t.Contains(e)) return false;
            return true;
        }

        public static bool Contains<T>(this IEnumerable<T> t, Predicate<T> p) {
            foreach (var e in t) {
                if (p(e)) {
                    return true;
                }
            }
            return false;
        }

        public static bool Contains(this IEnumerable t, Predicate<object> p) {
            foreach (var e in t) {
                if (p(e)) {
                    return true;
                }
            }
            return false;
        }

        public static T Find<T>(this IEnumerable<T> t, Predicate<T> p) {
            foreach (var e in t) {
                if (p(e)) {
                    return e;
                }
            }
            return default(T);
        }

        public static object Find(this IEnumerable t, Predicate<object> p) {
            foreach (var e in t) {
                if (p(e)) {
                    return e;
                }
            }
            return null;
        }

       

        public static bool ContainsAll<T>(this IEnumerable<T> t, IEnumerable p) {
            foreach (T e in p) if (!t.Contains(e)) return false;
            return true;
        }

        public static IOrderedEnumerable<T> OrderBy<T, TKey>(this IEnumerable<T> t, Func<T, TKey> keySelector, Comparison<TKey> comparison) {
            return t.OrderBy(keySelector, ZComparer.Create(comparison));
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> t, Comparison<T> comparison) {
            return t.OrderBy(k => k, ZComparer.Create(comparison));
        }

        public static IOrderedEnumerable<T> OrderByDescending<T, TKey>(this IEnumerable<T> t, Func<T, TKey> keySelector, Comparison<TKey> comparison) {
            return t.OrderBy(keySelector, ZComparer.Create(comparison));
        }

        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> t, Comparison<T> comparison) {
            return t.OrderBy(k => k, ZComparer.Create(comparison));
        }

        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable t) {
            Queue<T> q = new Queue<T>();
            foreach (T e in t) q.Enqueue(e);
            return q;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> t) {
            return !t.GetEnumerator().MoveNext();
        }

        public static bool IsEmpty(this IEnumerable t) {
            return !t.GetEnumerator().MoveNext();
        }

        private static void _QS<T>(T[] t, int l, int r, IComparer<T> comparer) {
            T x = t[(l + r) >> 1];
            int i=l,j=r;
            var c = comparer == null ? Comparer<T>.Default : comparer;
            do {
                while (c.Compare(t[i], x) < 0) ++i;
                while (c.Compare(t[j], x) > 0) --j;
                if (i <= j) {
                    var _ = t[i]; t[i] = t[j]; t[j] = _;
                    ++i; --j;
                }
            } while (i <= j);
            if (l < j) _QS(t, l, j, c);
            if (i < r) _QS(t, i, r, c);
        }

        public static T[] QuickSort<T>(this IEnumerable<T> t) {
            if (t == null) return null;
            if (t.IsEmpty()) return null;
            var tt=QuickSort(t, (Comparer<T>)null);
            return tt;
        }

        
        public static T[] QuickSort<T>(this IEnumerable<T> t, Comparison<T> comparison) {
            if (t == null) return null;
            if (t.IsEmpty()) return null;
            if (comparison == null) return QuickSort(t);
            else return QuickSort(t, new ZComparer<T>(comparison));
        }

        public static T[] QuickSort<T>(this IEnumerable<T> t, IComparer<T> comparer) {
            if (t == null) return null;
            var a = t.ToArray();
            _QS(a,0,a.Length-1,comparer);
            return a;
        }

        
        public static T[] HeapSort<T>(this IEnumerable<T> t, Comparison<T> comparison) {
            return t.ToPriorityQueue(comparison.Not()).ToArray();
        }

        public static T[] HeapSort<T>(this IEnumerable<T> t, IComparer<T> comparer) {
            return t.ToPriorityQueue(comparer.Not()).ToArray();
        }        

        public static T[] HeapSort<T>(this IEnumerable<T> t) {
            return t.ToPriorityQueue().ToArray();
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> t) {
            HashSet<T> hs = new HashSet<T>();
            foreach (var e in t) hs.Add(e);
            return hs;
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> t) {
            Queue<T> hs = new Queue<T>();
            foreach (var e in t) hs.Enqueue(e);
            return hs;
        }

        public static Stack<T> ToStack<T>(this IEnumerable<T> t) {
            Stack<T> hs = new Stack<T>();
            foreach (var e in t) hs.Push(e);
            return hs;
        }

        public static PriorityQueue<T> ToPriorityQueue<T>(this IEnumerable<T> t) {
            PriorityQueue<T> hs = new PriorityQueue<T>();
            foreach (var e in t) hs.Push(e);
            return hs;
        }

        public static PriorityQueue<T> ToPriorityQueue<T>(this IEnumerable<T> t,IComparer<T> comparer) {
            PriorityQueue<T> hs = new PriorityQueue<T>(comparer);
            foreach (var e in t) hs.Push(e);
            return hs;
        }

        public static PriorityQueue<T> ToPriorityQueue<T>(this IEnumerable<T> t, Comparison<T> comparison) {
            PriorityQueue<T> hs = new PriorityQueue<T>(comparison);
            foreach (var e in t) hs.Push(e);
            return hs;
        }
    }
}