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
using TRContent;

namespace TRWebBook {
    class ChapterCollection : IEnumerable<IContentItemAdapter>, INotifyCollectionChanged {
        private LinkedList<IContentItemAdapter> data;
        public ChapterCollection() {
            data = new LinkedList<IContentItemAdapter>();
        }

        public LinkedListNode<IContentItemAdapter> AddAfter(LinkedListNode<IContentItemAdapter> node, IContentItemAdapter item) {
            var r = data.AddAfter(node, item);
            item.Node = r;
            if (CollectionChanged!=null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return r;

        }

        public LinkedListNode<IContentItemAdapter> AddFirst(IContentItemAdapter item) {
            var r = data.AddFirst(item);
            item.Node = r;
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, 0));
            return r;
        }

        public LinkedListNode<IContentItemAdapter> AddLast(IContentItemAdapter item) {
            var r = data.AddLast(item);
            item.Node = r;
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, data.Count-1));
            return r;
        }

        public LinkedListNode<IContentItemAdapter> AddBefore(LinkedListNode<IContentItemAdapter> node, IContentItemAdapter item) {
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

        public IEnumerator<IContentItemAdapter> GetEnumerator() { return data.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return data.GetEnumerator(); }
        //IEnumerator Enumerable.GetEnumerator() { return data.GetEnumerator(); }
        //public IEnumerator<ContentItemAdapter> GetEnumerator() { return data.GetEnumerator(); }
        public bool Contains(IContentItemAdapter value) { return data.Contains(value); }
        public void CopyTo(IContentItemAdapter[] array, int index) { data.CopyTo(array, index); }
        public LinkedListNode<IContentItemAdapter> Find(IContentItemAdapter value) { return data.Find(value); }
        public LinkedListNode<IContentItemAdapter> FindLast(IContentItemAdapter value) { return data.FindLast(value); }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) { data.GetObjectData(info, context); }
        public virtual void OnDeserialization(object sender) { data.OnDeserialization(sender); }
        public void Remove(LinkedListNode<IContentItemAdapter> node) {
            data.Remove(node);
            Notify();
        }
        public bool Remove(IContentItemAdapter value) {
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
        public LinkedListNode<IContentItemAdapter> First { get { return data.First; } }
        public LinkedListNode<IContentItemAdapter> Last { get { return data.Last; } }
        public int Count { get { return data.Count; } }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Notify() {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
