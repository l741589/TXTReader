using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TXTReader.Data {
    public class ChapterCollection : IEnumerable<ContentItemAdapter>, INotifyCollectionChanged {
        private LinkedList<ContentItemAdapter> data;
        public ChapterCollection() {
            data = new LinkedList<ContentItemAdapter>();
        }

        public LinkedListNode<ContentItemAdapter> AddAfter(LinkedListNode<ContentItemAdapter> node, ContentItemAdapter item) {
            var r = data.AddAfter(node, item);
            item.Node = r;
            if (CollectionChanged!=null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return r;

        }

        public LinkedListNode<ContentItemAdapter> AddFirst(ContentItemAdapter item) {
            var r = data.AddFirst(item);
            item.Node = r;
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, 0));
            return r;
        }

        public LinkedListNode<ContentItemAdapter> AddLast(ContentItemAdapter item) {
            var r = data.AddLast(item);
            item.Node = r;
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, data.Count-1));
            return r;
        }

        public LinkedListNode<ContentItemAdapter> AddBefore(LinkedListNode<ContentItemAdapter> node, ContentItemAdapter item) {
            var r = data.AddBefore(node, item);
            item.Node = r;
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return r;
        }

        public void Clear() {
            data.Clear();
            Notify();
        }
        IEnumerator IEnumerable.GetEnumerator() { return data.GetEnumerator(); }
        IEnumerator<ContentItemAdapter> IEnumerable<ContentItemAdapter>.GetEnumerator() { return data.GetEnumerator(); }
        public bool Contains(ContentItemAdapter value) { return data.Contains(value); }
        public void CopyTo(ContentItemAdapter[] array, int index) { data.CopyTo(array, index); }
        public LinkedListNode<ContentItemAdapter> Find(ContentItemAdapter value) { return data.Find(value); }
        public LinkedListNode<ContentItemAdapter> FindLast(ContentItemAdapter value) { return data.FindLast(value); }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) { data.GetObjectData(info, context); }
        public virtual void OnDeserialization(object sender) { data.OnDeserialization(sender); }
        public void Remove(LinkedListNode<ContentItemAdapter> node) {
            data.Remove(node);
            Notify();
        }
        public bool Remove(ContentItemAdapter value) {
            var b = data.Remove(value);
            Notify();
            return b;
        }
        public void RemoveFirst() {                    
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, data.First.Value, 0));
            data.RemoveFirst();    
        }
        public void RemoveLast() {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, data.Last.Value, data.Count - 1));
            data.RemoveLast();
        }
        public LinkedListNode<ContentItemAdapter> First { get { return data.First; } }
        public LinkedListNode<ContentItemAdapter> Last { get { return data.Last; } }
        public int Count { get { return data.Count; } }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Notify() {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
