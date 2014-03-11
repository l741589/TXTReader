using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Utility;

namespace Zlib.Algorithm {
    public class PriorityQueue<T> : IEnumerable<T>{
        private IComparer<T> comparer;
        private T[] heap;

        public int Count { get; private set; }

        public PriorityQueue() : this((IComparer<T>)null) { }
        public PriorityQueue(int capacity) : this(capacity, (IComparer<T>)null) { }
        public PriorityQueue(IComparer<T> comparer) : this(16, comparer) { }
        public PriorityQueue(Comparison<T> comparison) : this(16, ZComparer.Create(comparison)) { }
        public PriorityQueue(int capacity, Comparison<T> comparison) : this(capacity, ZComparer.Create(comparison)) { }
        public PriorityQueue(int capacity, IComparer<T> comparer) {
            this.comparer = (comparer == null) ? Comparer<T>.Default : comparer;
            this.heap = new T[capacity];
        }

        public void Push(T v) {
            if (Count >= heap.Length) Array.Resize(ref heap, Count * 2);
            heap[Count] = v;
            SiftUp(Count++);
        }

        public T Pop() {
            var v = Top();
            heap[0] = heap[--Count];
            if (Count > 0) SiftDown(0);
            return v;
        }

        public T Top() {
            if (Count > 0) return heap[0];
            throw new InvalidOperationException("优先队列为空");
        }

        void SiftUp(int n) {
            var v = heap[n];
            for (var n2 = n / 2; n > 0 && comparer.Compare(v, heap[n2]) > 0; n = n2, n2 /= 2) heap[n] = heap[n2];
            heap[n] = v;
        }

        void SiftDown(int n) {
            var v = heap[n];
            for (var n2 = n * 2; n2 < Count; n = n2, n2 *= 2) {
                if (n2 + 1 < Count && comparer.Compare(heap[n2 + 1], heap[n2]) > 0) n2++;
                if (comparer.Compare(v, heap[n2]) >= 0) break;
                heap[n] = heap[n2];
            }
            heap[n] = v;
        }

        public Queue<T> ToQueue() {
            Queue<T> ss = new Queue<T>();
            while (Count > 0) {
                var t = Pop();
                ss.Enqueue(t);
            }
            foreach (var e in ss) Push(e);
            return ss;
        }

        public IEnumerator<T> GetEnumerator() {
            return ToQueue().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return ToQueue().GetEnumerator();
        }
    }
}
