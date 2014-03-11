using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zlib.Utility {
    public static class ComparerExtension {
        public static Comparison<T> Not<T>(this Comparison<T> comparison) {
            return (l, r) => comparison(r, l);
        }

        public static IComparer<T> Not<T>(this IComparer<T> comparer) {
            return ZComparer.Create<T>((l, r) => comparer.Compare(r, l));
        }

    }

    public class ZComparer : ZComparer<object> {
        public ZComparer(Comparison<object> com) :base(com){

        }
    }
    public class ZComparer<E> : IComparer<E> {
        private Comparison<E> com;
        public ZComparer(Comparison<E> com) {
            this.com = com;
        }

        public int Compare(E x, E y) {
            return com(x, y);
        }

        public static ZComparer<E> Default {
            get {
                return Create(Comparer<E>.Default);
            }
        }

        public static ZComparer<T> Create<T>(Comparer<T> com) {
            return new ZComparer<T>(com.Compare);
        }

        public static ZComparer<T> Create<T>(Comparison<T> com) {
            return new ZComparer<T>(com);
        }

        public static implicit operator ZComparer<E>(Comparison<E> com) {
            return Create(com);
        }
    }
}
