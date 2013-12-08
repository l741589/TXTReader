using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXTReader.Data {
    public class ChapterCollection : IEnumerable<ContentItemAdapter> {
        private LinkedList<ContentItemAdapter> data;
        public ChapterCollection() {
            data = new LinkedList<ContentItemAdapter>();
        }

        public LinkedListNode<ContentItemAdapter> AddAfter(LinkedListNode<ContentItemAdapter> node, ContentItemAdapter item) {
            var r = data.AddAfter(node, item);
            item.Node = r;
            return r;

        }

        public LinkedListNode<ContentItemAdapter> AddFirst(ContentItemAdapter item) {
            var r = data.AddFirst(item);
            item.Node = r;
            return r;
        }

        public LinkedListNode<ContentItemAdapter> AddLast(ContentItemAdapter item) {
            var r = data.AddLast(item);
            item.Node = r;
            return r;
        }

        public LinkedListNode<ContentItemAdapter> AddBefore(LinkedListNode<ContentItemAdapter> node, ContentItemAdapter item) {
            var r = data.AddBefore(node, item);
            item.Node = r;
            return r;
        }

        public void Clear() {
            data.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return data.GetEnumerator();
        }

        IEnumerator<ContentItemAdapter> IEnumerable<ContentItemAdapter>.GetEnumerator() {
            return data.GetEnumerator();
        }

        public LinkedListNode<ContentItemAdapter> First { get { return data.First; } }
        public LinkedListNode<ContentItemAdapter> Last { get { return data.Last; } }
    }
}
